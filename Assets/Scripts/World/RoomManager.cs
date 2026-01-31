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
    public GameObject combatRoomPrefab;
    public GameObject tribeRoomPrefabA;
    public GameObject tribeRoomPrefabB;
    public GameObject tribeRoomPrefabC;
    public GameObject bossRoomPrefab;

    [Header("Room Settings")]
    public Vector2 roomSize = new Vector2(18f, 10f);

    private Dictionary<Vector2Int, GameObject> rooms =
        new Dictionary<Vector2Int, GameObject>();

    private Vector2Int currentRoomIndex = Vector2Int.zero;
    private bool isTransitioning = false;
    public bool IsTransitioning => isTransitioning;
    private Door lastEnteredDoor;
    private Vector2Int previousRoomIndex;
    public Vector2Int CurrentRoomIndex => currentRoomIndex;
    public Vector2Int PreviousRoomIndex => previousRoomIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentRoomIndex = Vector2Int.zero;

        // Spawn the start room THROUGH the map system
        CreateRoom(currentRoomIndex);

        // Position camera correctly
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }

    public void MoveToRoom(DoorDirection direction)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        Vector2Int nextRoomIndex = currentRoomIndex;

        switch (direction)
        {
            case DoorDirection.Right: nextRoomIndex += Vector2Int.right; break;
            case DoorDirection.Left: nextRoomIndex += Vector2Int.left; break;
            case DoorDirection.Up: nextRoomIndex += Vector2Int.up; break;
            case DoorDirection.Down: nextRoomIndex += Vector2Int.down; break;
        }

        if (!WorldMap.Rooms.ContainsKey(nextRoomIndex))
        {
            isTransitioning = false;
            return;
        }

        if (!rooms.ContainsKey(nextRoomIndex))
        {
            CreateRoom(nextRoomIndex);
        }
        if (currentRoomIndex == Vector2Int.zero)
        {
            MusicManager.Instance.LeaveSpawnForever();
        }
        previousRoomIndex = currentRoomIndex;
        currentRoomIndex = nextRoomIndex;
        miniMap.UpdateCurrentRoom(currentRoomIndex);
        HandleRoomMusic(currentRoomIndex);
        Vector3 roomWorldPos = new Vector3(
            currentRoomIndex.x * roomSize.x,
            currentRoomIndex.y * roomSize.y,
            0f
        );

        mainCamera.transform.position = roomWorldPos + new Vector3(0, 0, -10);

        MovePlayerToEntrance(direction);

        Invoke(nameof(EndTransition), 0.05f);
    }

    private void MovePlayerToEntrance(DoorDirection direction)
    {
        Vector3 offset = Vector3.zero;

        switch (direction)
        {
            case DoorDirection.Right: offset = new Vector3(-7f, 0f, 0f); break;
            case DoorDirection.Left: offset = new Vector3(7f, 0f, 0f); break;
            case DoorDirection.Up: offset = new Vector3(0f, -4f, 0f); break;
            case DoorDirection.Down: offset = new Vector3(0f, 4f, 0f); break;
        }

        Vector3 newPos = mainCamera.transform.position + offset;
        newPos.z = -0.1f;

        player.position = newPos;
    }

    private void CreateRoom(Vector2Int index)
    {
        if (!WorldMap.Rooms.ContainsKey(index))
            return;

        RoomInfo info = WorldMap.Rooms[index];
        GameObject prefabToSpawn = null;

        switch (info.roomType)
        {
            case RoomType.Spawn:
                prefabToSpawn = startRoomPrefab;
                break;

            case RoomType.Enemy:
                prefabToSpawn = combatRoomPrefab;
                break;

            case RoomType.Tribe:
                prefabToSpawn = info.tribe switch
                {
                    TribeType.TribeA => tribeRoomPrefabA,
                    TribeType.TribeB => tribeRoomPrefabB,
                    TribeType.TribeC => tribeRoomPrefabC,
                    _ => tribeRoomPrefabA
                };
                break;

            case RoomType.Boss:
                prefabToSpawn = bossRoomPrefab;
                break;
        }

        Vector3 roomWorldPos = new Vector3(
            index.x * roomSize.x,
            index.y * roomSize.y,
            0f
        );

        GameObject room = Instantiate(prefabToSpawn, roomWorldPos, Quaternion.identity);
        rooms.Add(index, room);

        SetupDoors(room, index);
    }

    private void SetupDoors(GameObject room, Vector2Int index)
    {
        Door[] doors = room.GetComponentsInChildren<Door>(true);

        foreach (Door door in doors)
        {
            // ALWAYS disable first
            door.gameObject.SetActive(false);

            Vector2Int neighbor = index;

            switch (door.direction)
            {
                case DoorDirection.Up: neighbor += Vector2Int.up; break;
                case DoorDirection.Down: neighbor += Vector2Int.down; break;
                case DoorDirection.Left: neighbor += Vector2Int.left; break;
                case DoorDirection.Right: neighbor += Vector2Int.right; break;
            }

            if (WorldMap.Rooms.ContainsKey(neighbor))
            {
                door.gameObject.SetActive(true);
            }
        }
    }

    private void HandleRoomMusic(Vector2Int roomIndex)
    {
        if (!WorldMap.Rooms.ContainsKey(roomIndex))
            return;

        RoomInfo info = WorldMap.Rooms[roomIndex];

        if (info.roomType == RoomType.Tribe)
        {
            MusicManager.Instance.EnterTribe(info.tribe);
        }
        else
        {
            MusicManager.Instance.ExitTribe();
        }
    }
    public void SetDoorsLocked(Vector2Int roomIndex, bool locked)
    {
        if (!rooms.ContainsKey(roomIndex))
            return;

        Door[] doors = rooms[roomIndex].GetComponentsInChildren<Door>(true);

        foreach (Door door in doors)
        {
            door.SetLocked(locked);
        }
    }
    private void EndTransition()
    {
        isTransitioning = false;
    }
}
