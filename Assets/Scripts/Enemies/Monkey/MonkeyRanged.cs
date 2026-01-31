using UnityEngine;

public class MonkeyRanged : MonoBehaviour
{
    public float attackRange = 7f;
    public float throwCooldown = 2f;
    public float spawnAttackDelay = 1f;

    float spawnTime;
    float lastThrowTime;

    public GameObject bananaPrefab;
    public Transform throwPoint;
    public float bananaSpeed = 8f;

    Transform player;
    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTime = Time.time;
    }

    void Update()
    {
        if (player == null)
            return;

        if (Time.time - spawnTime < spawnAttackDelay)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange &&
            Time.time >= lastThrowTime + throwCooldown)
        {
            StartAttack();
            lastThrowTime = Time.time;
        }

        FacePlayer();
    }

    void StartAttack()
    {
        if (anim != null)
            anim.SetTrigger("Attack");
    }

    // 🔥 CALLED BY ANIMATION EVENT
    public void ThrowBanana()
    {
        if (bananaPrefab == null || throwPoint == null || player == null)
            return;

        Vector2 direction = (player.position - throwPoint.position).normalized;

        GameObject banana = Instantiate(
            bananaPrefab,
            throwPoint.position,
            Quaternion.identity
        );

        BananaProjectile proj = banana.GetComponent<BananaProjectile>();
        if (proj != null)
            proj.Launch(direction, bananaSpeed, ProjectileOwner.Enemy);
    }

    void FacePlayer()
    {
        Vector3 scale = transform.localScale;
        float baseX = Mathf.Abs(scale.x);

        scale.x = player.position.x < transform.position.x
            ? -baseX
            : baseX;

        transform.localScale = scale;
    }
}
