using UnityEngine;

public class Dart : MonoBehaviour
{
    public int damage = 1;
    public float speed = 12f;
    public float lifetime = 2f;
    public float momentumFactor = 0.3f;
    MaskEffectType dartEffect;

    [Header("Ricochet")]
    public float ricochetSearchRadius = 6f;

    [Header("Hit Flash FX")]
    public GameObject hitFlashPrefab; // For ice/poison legacy FX

    public Color iceColor = new Color(0.6f, 0.85f, 1f);
    public Color poisonColor = new Color(0.3f, 0.9f, 0.3f);
    public Color fireColor = new Color(1f, 0.4f, 0.1f);

    Rigidbody2D rb;
    int remainingBounces;
    bool piercing;

    IDamageable lastHitEnemy;

    public void InitFromMask()
    {
        dartEffect = MaskManager.Instance.ElementalEffect;
        piercing = dartEffect == MaskEffectType.Piercing;
        remainingBounces = dartEffect == MaskEffectType.Ricochet ? 1 : 0;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        dartEffect = MaskManager.Instance.DartEffect;
        piercing = dartEffect == MaskEffectType.Piercing;
        remainingBounces = dartEffect == MaskEffectType.Ricochet ? 1 : 0;

        Destroy(gameObject, lifetime);
    }

    public void Fire(Vector2 shootDir, Vector2 inheritedVelocity)
    {
        Vector2 finalVelocity =
            shootDir.normalized * speed +
            inheritedVelocity * momentumFactor;

        rb.linearVelocity = finalVelocity;
        Rotate(finalVelocity);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
            return;

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
        SpawnHitFlash(other);

        if (dartEffect == MaskEffectType.Ice)
        {
            enemy.GetTransform()
                 .GetComponent<EnemyStatus>()
                 ?.ApplyIceSlow(0f, 0f);
        }

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

  void SpawnHitFlash(Collider2D hitCollider)
{
    Vector3 hitPos = hitCollider.bounds.center;
    float scale = GetColliderSize(hitCollider);

    if (DartFXManager.Instance != null)
    {
        DartFXManager.Instance.SpawnFX(hitPos, dartEffect, scale);
    }
    else if (hitFlashPrefab != null)
    {
        // Legacy fallback
        GameObject fx = Instantiate(hitFlashPrefab, hitPos, Quaternion.identity);
        HitFlashFX fxComp = fx.GetComponent<HitFlashFX>();
        if (fxComp != null)
        {
            Color color = dartEffect switch
            {
                MaskEffectType.Ice => iceColor,
                MaskEffectType.Poison => poisonColor,
                MaskEffectType.Fire => fireColor,
                _ => Color.white
            };
            fx.transform.localScale = Vector3.one * scale;
            fxComp.Play(color);
        }

        if (fx.GetComponent<FXSelfDestruct>() == null)
            fx.AddComponent<FXSelfDestruct>();
    }
}



    float GetColliderSize(Collider2D col)
    {
        if (col is CircleCollider2D c)
            return c.radius * 2f;

        if (col is BoxCollider2D b)
            return Mathf.Max(b.size.x, b.size.y);

        return 0.8f;
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
