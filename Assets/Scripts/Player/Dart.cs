using UnityEngine;

public class Dart : MonoBehaviour
{
    public int damage = 1;
    public float speed = 12f;
    public float lifetime = 2f;
    public float momentumFactor = 0.3f;

    [Header("Ricochet")]
    public float ricochetSearchRadius = 6f;
    Rigidbody2D rb;

    int remainingBounces;
    bool piercing;

    IDamageable lastHitEnemy;// 🔑 THIS IS CRITICAL

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void InitFromMask()
    {
        var effect = MaskManager.Instance.DartEffect;
        piercing = effect == MaskEffectType.Piercing;
        remainingBounces = effect == MaskEffectType.Ricochet ? 1 : 0;
    }
    public void Fire(Vector2 shootDir, Vector2 inheritedVelocity)
    {
        Vector2 finalVelocity =
            shootDir.normalized * speed +
            inheritedVelocity * momentumFactor;

        rb.linearVelocity = finalVelocity;
        Rotate(finalVelocity);
    }
    void RotateToDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable enemy = other.GetComponentInParent<IDamageable>();
        if (enemy == null)
        {
            if (other.CompareTag("Wall"))
                Destroy(gameObject);
            return;
        }

        if (enemy == lastHitEnemy)
            return;

        lastHitEnemy = enemy;

        enemy.TakeDamage(damage, rb.linearVelocity);

        if (piercing)
            return;

        if (remainingBounces > 0)
        {
            remainingBounces--;

            Vector2 dir = FindAnotherEnemy(enemy);

            if (dir == Vector2.zero)
            {
                Destroy(gameObject);
                return;
            }

            rb.linearVelocity = dir * speed;
            Rotate(dir);
            return;
        }

        Destroy(gameObject);
    }


    Vector2 FindAnotherEnemy(IDamageable hitEnemy)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ricochetSearchRadius);

        IDamageable best = null;
        float bestDist = float.MaxValue;

        foreach (var h in hits)
        {
            IDamageable dmg = h.GetComponentInParent<IDamageable>();
            if (dmg == null || dmg == hitEnemy)
                continue;

            float d = ((Vector2)dmg.GetTransform().position - (Vector2)transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = dmg;
            }
        }

        if (best != null)
            return ((Vector2)best.GetTransform().position - (Vector2)transform.position).normalized;

        return Vector2.zero;
    }


    void Rotate(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
