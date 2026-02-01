using UnityEngine;

public class BearMinionChase : MonoBehaviour
{
    public float moveSpeed = 2.2f;
    public int contactDamage = 1;
    public float contactCooldown = 1f;

    // --- Knockback (added) ---
    public float knockbackForce = 6f;
    bool allowKnockback;
    float knockbackTimer;

    Transform player;
    Rigidbody2D rb;
    float lastContactTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        // Knockback timer
        if (allowKnockback)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                allowKnockback = false;
            return; // ⛔ DO NOT apply chase movement
        }

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        FacePlayer(dir);
    }

    void FacePlayer(Vector2 dir)
    {
        if (dir.x == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (Time.time < lastContactTime + contactCooldown)
            return;

        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(contactDamage);

        lastContactTime = Time.time;
    }

    // ---------------- KNOCKBACK (added) ----------------

    public void AllowKnockback(float duration)
    {
        allowKnockback = true;
        knockbackTimer = duration;
    }
    public void ApplyKnockback(Vector2 hitDirection, float duration = 0.08f)
    {
        allowKnockback = true;
        knockbackTimer = duration;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
