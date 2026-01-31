using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{
    private RoomEnemySpawner spawner;

    void Awake()
    {
        spawner = GetComponentInParent<RoomEnemySpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (spawner != null)
        {
            spawner.SpawnEnemies();
        }
    }
}
