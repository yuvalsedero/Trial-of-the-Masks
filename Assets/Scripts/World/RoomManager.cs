using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Player & Camera")]
    public Transform player;
    public Camera mainCamera;

    [Header("Prefabs")]
    public MiniMapManager miniMap;

    public GameObject startRoomPrefab;

    [Header("Combat Room Variants")]
    public GameObject[] combatRoomPrefabs; // size = 3

    public GameObject tribeRoomPrefabA;
    public GameObject tribeRoomPrefabB;
    public GameObject tribeRoomPrefabC;
    public GameObject bossRoomPrefab;

    [Header("Room Settings")]
    public Vector2 roomSize = new Vector2(18f, 10f);

    private Dictionary<Vector2Int, GameObject> rooms = new();
    private Vector2Int currentRoomIndex = Vector2Int.zero;
    private Vector2Int previousRoomIndex = Vector2Int.zero;

    private bool isTransitioning = false;
    public bool IsTransitioning => isTransitioning;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentRoomIndex = Vector2Int.zero;
        CreateRoom(currentRoomIndex);

        mainCamera.transform.position = new Vector3(0, 0, -10);
        miniMap.UpdateCurrentRoom(currentRoomIndex);
    }

    public void MoveToRoom(DoorDirection direction)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        Vector2Int nextRoomIndex = currentRoomIndex + direction switch
        {
            DoorDirection.Right => Vector2Int.right,
            DoorDirection.Left => Vector2Int.left,
            DoorDirection.Up => Vector2Int.up,
            DoorDirection.Down => Vector2Int.down,
            _ => Vector2Int.zero
        };

        if (!WorldMap.Rooms.ContainsKey(nextRoomIndex))
        {
            isTransitioning = false;
            return;
        }

        if (!rooms.ContainsKey(nextRoomIndex))
            CreateRoom(nextRoomIndex);

        previousRoomIndex = currentRoomIndex;
        currentRoomIndex = nextRoomIndex;

        // ✅ LEAVE SPAWN FOREVER (ONCE)
        if (previousRoomIndex == Vector2Int.zero)
            MusicManager.Instance.LeaveSpawnForever();

        miniMap.UpdateCurrentRoom(currentRoomIndex);
        HandleRoomMusic(currentRoomIndex);

        Vector3 camPos = new Vector3(
            currentRoomIndex.x * roomSize.x,
            currentRoomIndex.y * roomSize.y,
            -10f
        );

        mainCamera.transform.position = camPos;
        MovePlayerToEntrance(direction);

        Invoke(nameof(EndTransition), 0.05f);
    }

    void EndTransition() => isTransitioning = false;

    void MovePlayerToEntrance(DoorDirection direction)
    {
        Vector3 offset = direction switch
        {
            DoorDirection.Right => new Vector3(-7f, 0, 0),
            DoorDirection.Left => new Vector3(7f, 0, 0),
            DoorDirection.Up => new Vector3(0, -4f, 0),
            DoorDirection.Down => new Vector3(0, 4f, 0),
            _ => Vector3.zero
        };

        Vector3 pos = mainCamera.transform.position + offset;
        pos.z = -0.1f;
        player.position = pos;
    }

    void CreateRoom(Vector2Int index)
    {
        if (!WorldMap.Rooms.TryGetValue(index, out RoomInfo info))
            return;

        GameObject prefab = null;

        switch (info.roomType)
        {
            case RoomType.Spawn:
                prefab = startRoomPrefab;
                break;

            case RoomType.Enemy:
                prefab = combatRoomPrefabs[Random.Range(0, combatRoomPrefabs.Length)];
                break;

            case RoomType.Tribe:
                prefab = info.tribe switch
                {
                    TribeType.TribeA => tribeRoomPrefabA,
                    TribeType.TribeB => tribeRoomPrefabB,
                    TribeType.TribeC => tribeRoomPrefabC,
                    _ => tribeRoomPrefabA
                };
                break;

            case RoomType.Boss:
                prefab = bossRoomPrefab;
                break;
        }

        Vector3 worldPos = new(
            index.x * roomSize.x,
            index.y * roomSize.y,
            0f
        );

        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);
        rooms.Add(index, room);
        SetupDoors(room, index);

        // spawn enemies if needed
        RoomEnemySpawner spawner = room.GetComponentInChildren<RoomEnemySpawner>();
        if (spawner != null)
            spawner.SpawnEnemies();
    }

    void SetupDoors(GameObject room, Vector2Int index)
    {
        Door[] doors = room.GetComponentsInChildren<Door>(true);

        foreach (Door door in doors)
        {
            door.gameObject.SetActive(false);

            Vector2Int neighbor = index + door.direction switch
            {
                DoorDirection.Up => Vector2Int.up,
                DoorDirection.Down => Vector2Int.down,
                DoorDirection.Left => Vector2Int.left,
                DoorDirection.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };

            if (WorldMap.Rooms.ContainsKey(neighbor))
                door.gameObject.SetActive(true);
        }
    }

    private void HandleRoomMusic(Vector2Int roomIndex)
    {
        if (!WorldMap.Rooms.TryGetValue(roomIndex, out RoomInfo info))
            return;

        Debug.Log($"[ROOM] Index={roomIndex} Type={info.roomType} Tribe={info.tribe}");

        switch (info.roomType)
        {
            case RoomType.Tribe:
                Debug.Log($"[MUSIC] Enter tribe {info.tribe}");
                MusicManager.Instance.EnterTribe(info.tribe);
                break;

            case RoomType.Boss:
                Debug.Log("[MUSIC] Enter BOSS");
                MusicManager.Instance.EnterBoss();
                break;

            default:
                Debug.Log("[MUSIC] Exit tribe / boss");
                MusicManager.Instance.ExitTribe();
                MusicManager.Instance.ExitBoss();
                break;
        }
    }


    public void SetDoorsLocked(Vector2Int roomIndex, bool locked)
    {
        if (!rooms.ContainsKey(roomIndex))
            return;

        foreach (Door door in rooms[roomIndex].GetComponentsInChildren<Door>(true))
            door.SetLocked(locked);
    }
}
