using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    public GameObject boarPrefab;
    public GameObject monkeyPrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 3f;
    public float minDistanceFromPlayer = 2.5f;
    public int maxSpawnAttempts = 20;

    private int aliveEnemies;
    private Vector2Int roomIndex;
    private bool isCleared = false;
    private bool hasSpawned = false;

    private Transform player;

    void Awake()
    {
        Vector3 pos = transform.position;
        roomIndex = new Vector2Int(
            Mathf.RoundToInt(pos.x / RoomManager.Instance.roomSize.x),
            Mathf.RoundToInt(pos.y / RoomManager.Instance.roomSize.y)
        );

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[RoomEnemySpawner] Player not found. Distance check disabled.");
    }

    public void SpawnEnemies()
    {
        if (hasSpawned || isCleared)
            return;

        if (!WorldMap.Rooms.ContainsKey(roomIndex))
            return;

        RoomInfo info = WorldMap.Rooms[roomIndex];
        if (info.roomType != RoomType.Enemy)
            return;

        aliveEnemies = info.boarCount + info.monkeyCount;

        if (aliveEnemies <= 0)
            return;

        hasSpawned = true;

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

        Vector3 spawnPos = Vector3.zero;
        bool foundValidPosition = false;

        Vector2 roomSize = RoomManager.Instance.roomSize;
        Vector3 roomCenter = transform.position;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = roomCenter + (Vector3)randomOffset;

            // --- Check room bounds ---
            bool insideRoom =
                Mathf.Abs(candidate.x - roomCenter.x) <= roomSize.x * 0.5f &&
                Mathf.Abs(candidate.y - roomCenter.y) <= roomSize.y * 0.5f;

            if (!insideRoom)
                continue;

            // --- Check distance from player ---
            if (player != null)
            {
                float distance = Vector2.Distance(candidate, player.position);
                if (distance < minDistanceFromPlayer)
                    continue;
            }

            spawnPos = candidate;
            foundValidPosition = true;
            break;
        }

        // Fallback if no valid position found
        if (!foundValidPosition)
        {
            spawnPos = roomCenter;
            Debug.LogWarning("[RoomEnemySpawner] Could not find safe spawn position, using room center.");
        }

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

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
