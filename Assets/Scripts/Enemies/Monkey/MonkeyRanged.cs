using UnityEngine;

public class MonkeyRanged : MonoBehaviour
{
    public float attackRange = 7f;
    public float throwCooldown = 2f;
    public float spawnAttackDelay = 1f;
    private float spawnTime; public GameObject bananaPrefab;
    public Transform throwPoint;

    public float bananaSpeed = 8f;

    Transform player;
    float lastThrowTime;
    public AudioClip[] throwSounds; // add as many as you want in Inspector
private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTime = Time.time; // 👈 NEW
        audioSource = GetComponent<AudioSource>();
if (audioSource == null)
    audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null)
            return;

        if (Time.time - spawnTime < spawnAttackDelay)
            return; // 👈 NEW
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time >= lastThrowTime + throwCooldown)
        {
            ThrowBanana();
            lastThrowTime = Time.time;
        }

        FacePlayer();
    }

    void ThrowBanana()
    {
        if (bananaPrefab == null || throwPoint == null)
            return;

        Vector2 direction = (player.position - throwPoint.position).normalized;

        GameObject banana = Instantiate(bananaPrefab, throwPoint.position, Quaternion.identity);
        BananaProjectile proj = banana.GetComponent<BananaProjectile>();

        if (proj != null)
            proj.Launch(direction, bananaSpeed, ProjectileOwner.Enemy);
            if (throwSounds != null && throwSounds.Length > 0)
{
    AudioClip clip = throwSounds[Random.Range(0, throwSounds.Length)];
    audioSource.PlayOneShot(clip);
}
    }


    void FacePlayer()
    {
        Vector3 scale = transform.localScale;
        float baseX = Mathf.Abs(scale.x); // preserve fatness

        scale.x = player.position.x < transform.position.x
            ? -baseX
            : baseX;

        transform.localScale = scale;
    }
}
