using UnityEngine;

public class BearBossSummon : MonoBehaviour
{
    public GameObject minionPrefab;
    public Transform[] summonPoints;

    [Header("Counts")]
    public int phase1Amount = 1;
    public int phase2Amount = 3;

    [Header("Limits")]
    public int maxMinionsAlive = 6;

    int aliveMinions = 0;

    // Controller calls this every 15 seconds
    public void Summon(bool phaseTwo)
    {
        if (minionPrefab == null || summonPoints == null || summonPoints.Length == 0)
            return;

        int wantToSpawn = phaseTwo ? phase2Amount : phase1Amount;

        // clamp to max alive
        int canSpawn = maxMinionsAlive - aliveMinions;
        int spawnCount = Mathf.Clamp(wantToSpawn, 0, canSpawn);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform point = summonPoints[Random.Range(0, summonPoints.Length)];
            GameObject minion = Instantiate(minionPrefab, point.position, Quaternion.identity);

            aliveMinions++;

            // when minion dies -> decrement aliveMinions
            // If your minion uses BoarHealth:
            BoarHealth bh = minion.GetComponent<BoarHealth>();
            if (bh != null)
            {
                bh.OnDeath += OnMinionDeath;
            }
            else
            {
                // fallback: if it's not a boar, still avoid permanent count lock
                StartCoroutine(CountSafety(minion));
            }
        }
    }

    void OnMinionDeath()
    {
        aliveMinions = Mathf.Max(0, aliveMinions - 1);
    }

    // Safety so count doesn't get stuck if prefab has different health script
    System.Collections.IEnumerator CountSafety(GameObject minion)
    {
        // wait until destroyed
        while (minion != null)
            yield return null;

        OnMinionDeath();
    }
}
