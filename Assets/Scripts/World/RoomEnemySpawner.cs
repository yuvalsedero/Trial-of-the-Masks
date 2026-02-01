using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    public GameObject boarPrefab;
    public GameObject monkeyPrefab;

    public float spawnRadius = 3f;

    private int aliveEnemies;
    private Vector2Int roomIndex;
    private bool isCleared = false;
    private bool hasSpawned = false;

    void Awake()
    {
        Vector3 pos = transform.position;
        roomIndex = new Vector2Int(
            Mathf.RoundToInt(pos.x / RoomManager.Instance.roomSize.x),
            Mathf.RoundToInt(pos.y / RoomManager.Instance.roomSize.y)
        );
    }

    public void SpawnEnemies()
    {
        if (hasSpawned)
            return;

        hasSpawned = true;

        if (isCleared)
            return;

        if (!WorldMap.Rooms.ContainsKey(roomIndex))
            return;

        RoomInfo info = WorldMap.Rooms[roomIndex];
        if (info.roomType != RoomType.Enemy)
            return;

        aliveEnemies = info.boarCount + info.monkeyCount;

        if (aliveEnemies <= 0)
            return;

        RoomManager.Instance.SetDoorsLocked(roomIndex, true);

        for (int i = 0; i < info.boarCount; i++)
            SpawnEnemy(boarPrefab);

        for (int i = 0; i < info.monkeyCount; i++)
            SpawnEnemy(monkeyPrefab);
    }


    void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[RoomEnemySpawner] Missing enemy prefab!");
            return;
        }

        GameObject enemy = Instantiate(
            prefab,
            transform.position + (Vector3)Random.insideUnitCircle * spawnRadius,
            Quaternion.identity
        );

        // ✅ Works for both boars + monkeys if they share BoarHealth (they currently do in your project)
        BoarHealth health = enemy.GetComponent<BoarHealth>();
        if (health != null)
            health.OnDeath += OnEnemyDied;
        else
            Debug.LogWarning("[RoomEnemySpawner] Spawned enemy has no BoarHealth, doors may never unlock.");
    }

    void OnEnemyDied()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            isCleared = true;
            RoomManager.Instance.SetDoorsLocked(roomIndex, false);
        }
    }
}
