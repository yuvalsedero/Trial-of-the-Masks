using UnityEngine;
using System.Collections;

public class BoarChase : MonoBehaviour
{
    float knockbackGrace = 0f;
    public float baseSpeed = 1.5f;
    float currentSpeed;
    Animator anim;
    public float spawnChargeDelay = 0.3f;
    private float spawnTime;
    public float chargeSpeed = 6f;
    public float windupTime = 0.6f;
    public float chargeTime = 0.6f;
    public float recoveryTime = 0.5f;

    public float minChargeInterval = 3f;
    public float maxChargeInterval = 5f;

    public float separationRadius = 1.2f;
    public float separationStrength = 1.5f;

    public Color chargeColor = new Color(1f, 0.7f, 0.25f);

    Rigidbody2D rb;
    Transform player;
    SpriteRenderer sr;
    Color baseColor;

    float timer;
    float nextChargeTime;
    Vector2 chargeDirection;

    [SerializeField] AudioClip[] chargeSounds;
    AudioSource audioSource;

    enum BoarState
    {
        Run,
        Windup,
        Charge,
        Recovery
    }

    BoarState state = BoarState.Run;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ScheduleNextCharge();
        spawnTime = Time.time; // 👈 NEW
    }

    void Update()
    {
        if (player == null)
            return;

        timer += Time.deltaTime;

        switch (state)
        {
            case BoarState.Run:
                if (Time.time - spawnTime < spawnChargeDelay)
                    break;

                if (timer >= nextChargeTime)
                    EnterWindup();
                break;

            case BoarState.Windup:
                if (timer >= windupTime)
                    EnterCharge();
                break;

            case BoarState.Charge:
                if (timer >= chargeTime)
                    EnterRecovery();
                break;

            case BoarState.Recovery:
                if (timer >= recoveryTime)
                    EnterRun();
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        // 👇 ALWAYS face player, except mid-charge
        if (state != BoarState.Charge)
        {
            FacePlayer();
        }
        if (knockbackGrace > 0f)
        {
            knockbackGrace -= Time.fixedDeltaTime;
            return; // allow knockback to play out
        }
        switch (state)
        {
            case BoarState.Run:
                Vector2 toPlayer = (player.position - transform.position).normalized;
                Vector2 separation = ComputeSeparation();
                rb.linearVelocity = (toPlayer + separation * separationStrength).normalized * currentSpeed;
                break;

            case BoarState.Windup:
                rb.linearVelocity = Vector2.zero;
                break;

            case BoarState.Charge:
                rb.linearVelocity = chargeDirection * chargeSpeed;
                break;
        }
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 scale = transform.localScale;
        float baseX = Mathf.Abs(scale.x);

        scale.x = player.position.x < transform.position.x
            ? -baseX
            : baseX;

        transform.localScale = scale;
    }

    // ---------- SLOW EFFECT ----------
    public void ApplySlow(float multiplier, float duration)
    {
        StopCoroutine(nameof(SlowRoutine));
        StartCoroutine(SlowRoutine(multiplier, duration));
    }

    IEnumerator SlowRoutine(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        currentSpeed = baseSpeed;
    }
    public void AllowKnockback(float time)
    {
        knockbackGrace = time;
    }
    // ---------- STATE ----------
    void EnterRun()
    {
        state = BoarState.Run;
        timer = 0f;
        SetChargeColor(false);
        ScheduleNextCharge();
    }

    void EnterWindup()
    {
        state = BoarState.Windup;
        timer = 0f;
        rb.linearVelocity = Vector2.zero;
        SetChargeColor(true);

        if (anim != null)
            anim.SetBool("IsWindup", true);
    }


    void EnterCharge()
    {
        state = BoarState.Charge;
        timer = 0f;
        chargeDirection = (player.position - transform.position).normalized;
        if (audioSource != null && chargeSounds != null && chargeSounds.Length > 0)
{
    AudioClip clip = chargeSounds[Random.Range(0, chargeSounds.Length)];
    audioSource.pitch = Random.Range(0.9f, 1.1f);
    audioSource.PlayOneShot(clip);
}

        if (anim != null)
            anim.SetBool("IsWindup", false); // back to Run anim
            
    }

    void EnterRecovery()
    {
        state = BoarState.Recovery;
        timer = 0f;
        SetChargeColor(false);
    }

    void ScheduleNextCharge()
    {
        nextChargeTime = Random.Range(minChargeInterval, maxChargeInterval);
    }

    void SetChargeColor(bool charging)
    {
        if (sr != null)
            sr.color = charging ? chargeColor : baseColor;
    }
    public void ModifySpeed(float multiplier)
    {
        currentSpeed = baseSpeed * Mathf.Clamp(multiplier, 0.1f, 1f);
    }
    Vector2 ComputeSeparation()
    {
        Vector2 separation = Vector2.zero;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (Collider2D col in nearby)
        {
            if (col.gameObject == gameObject)
                continue;

            BoarChase other = col.GetComponent<BoarChase>();
            if (other == null)
                continue;

            Vector2 away = (Vector2)(transform.position - other.transform.position);
            float dist = away.magnitude;
            if (dist > 0.01f)
                separation += away.normalized / dist;
        }

        return separation;
    }
}
