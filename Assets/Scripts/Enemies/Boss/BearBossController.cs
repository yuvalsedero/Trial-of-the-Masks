using UnityEngine;

public class BearBossController : MonoBehaviour
{
    [Header("Phase 1 Timers")]
    public float throwIntervalPhase1 = 5f;
    public float summonIntervalPhase1 = 15f;

    [Header("Phase 2 Timers")]
    public float throwIntervalPhase2 = 3f;
    public float summonIntervalPhase2 = 15f;

    float currentThrowInterval;
    float currentSummonInterval;

    float throwTimer;
    float summonTimer;

    bool phaseTwo = false;

    Transform player;

    BearBossMovement movement;
    BearBossThrow thrower;
    BearBossSummon summoner;
    BearBossHealth health;

    void Awake()
    {
        movement = GetComponent<BearBossMovement>();
        thrower = GetComponent<BearBossThrow>();
        summoner = GetComponent<BearBossSummon>();
        health = GetComponent<BearBossHealth>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // start in phase 1
        currentThrowInterval = throwIntervalPhase1;
        currentSummonInterval = summonIntervalPhase1;

        health.OnPhaseTwo += EnterPhaseTwo;
    }

    void Update()
    {
        if (player == null)
            return;

        // movement always active
        movement.MoveTowards(player.position);

        // --- THROW TIMER ---
        throwTimer += Time.deltaTime;
        if (throwTimer >= currentThrowInterval)
        {
            thrower.TryThrow(player.position);
            throwTimer = 0f;
        }

        // --- SUMMON TIMER ---
        summonTimer += Time.deltaTime;
        if (summonTimer >= currentSummonInterval)
        {
            summoner.Summon(phaseTwo);
            summonTimer = 0f;
        }
    }

    void EnterPhaseTwo()
    {
        phaseTwo = true;

        // speed up
        movement.moveSpeed += 1f;

        // faster attacks
        currentThrowInterval = throwIntervalPhase2;
        currentSummonInterval = summonIntervalPhase2;

        Debug.Log("Boss entered PHASE 2");
    }
}
