using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject dartPrefab;
    public float fireCooldown = 0.3f;
    public float doubleShotAngle = 15f;

    private Animator animator;
    private PlayerFacing facing;
    private float lastFireTime;
    private Vector2 shootDirection;
    private Rigidbody2D rb;

    public AudioClip[] shootSounds; // add as many as you want in Inspector
private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        facing = GetComponent<PlayerFacing>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
if (audioSource == null)
    audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        ReadShootInput();
        HandleShooting();
    }

    void ReadShootInput()
    {
        shootDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            shootDirection = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            shootDirection = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow))
            shootDirection = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            shootDirection = Vector2.right;
    }

    void HandleShooting()
    {
        if (shootDirection == Vector2.zero)
            return;

        if (Time.time < lastFireTime + fireCooldown)
            return;

        Shoot(shootDirection);
        lastFireTime = Time.time;
    }

    void Shoot(Vector2 direction)
    {
        if (direction.x != 0)
            facing.LockFacingForShot(direction.x);

        animator.SetTrigger("Attack");
        
        if (shootSounds != null && shootSounds.Length > 0)
{
    AudioClip clip = shootSounds[Random.Range(0, shootSounds.Length)];
    audioSource.PlayOneShot(clip);
}

        if (MaskManager.Instance.DartEffect == MaskEffectType.DoubleShot)
        {
            FireDouble(direction);
        }
        else
        {
            FireSingle(direction);
        }
    }

    void FireSingle(Vector2 direction)
    {
        SpawnDart(direction);
    }

    void FireDouble(Vector2 direction)
    {
        Vector2 perp = Vector2.Perpendicular(direction).normalized;
        float offset = 0.15f; // distance between the two darts

        SpawnDartFromOffset(direction, perp * offset);
        SpawnDartFromOffset(direction, -perp * offset);
    }
    void SpawnDartFromOffset(Vector2 direction, Vector2 offset)
    {
        Vector3 spawnPos =
            transform.position +
            (Vector3)(direction.normalized * 0.6f) +
            (Vector3)offset;

        GameObject dartObj = Instantiate(dartPrefab, spawnPos, Quaternion.identity);
        Dart dart = dartObj.GetComponent<Dart>();

        dart.InitFromMask();
        var stats = GetComponent<PlayerCombatStats>();
        if (stats != null)
        {
            dart.damage += stats.bonusDamage;
        }
        Vector2 inheritedVelocity = rb.linearVelocity;
        dart.Fire(direction, inheritedVelocity);
    }

    void SpawnDart(Vector2 direction)
    {
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * 0.6f);

        GameObject dartObj = Instantiate(dartPrefab, spawnPos, Quaternion.identity);
        Dart dart = dartObj.GetComponent<Dart>();

        dart.InitFromMask();
        var stats = GetComponent<PlayerCombatStats>();
        if (stats != null)
        {
            dart.damage += stats.bonusDamage;
        }
        Vector2 inheritedVelocity = rb.linearVelocity;
        dart.Fire(direction, inheritedVelocity);
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }

    public void OnAttackEnd()
    {
        facing.UnlockFacing();
    }
}
