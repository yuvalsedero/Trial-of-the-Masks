using UnityEngine;
using System;
using System.Collections;

public class BoarHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 3;
    int currentHealth;

    public Action OnDeath;

    public float knockbackForce = 6f;
    public float flashTime = 0.1f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Color originalColor;

    public GameObject meatPickupPrefab;
    public Sprite[] meatSprites;

    bool fireAppliedThisHit = false;

    Coroutine poisonCoroutine;
    Coroutine flashCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }

    void LateUpdate()
    {
        fireAppliedThisHit = false;

    }
    public Transform GetTransform()
    {
        return transform;
    }
    // ---------------- DAMAGE ----------------

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        currentHealth -= amount;

        // ❄ ICE
        if (MaskManager.Instance.ElementalEffect == MaskEffectType.Ice)
        {
            GetComponent<BoarChase>()?.ApplySlow(0.5f, 3f);
        }

        // ☠ POISON (refresh, no stack)
        if (MaskManager.Instance.ElementalEffect == MaskEffectType.Poison)
        {
            ApplyPoison(1, 3f, 1f);
        }

        // 🔥 FIRE
        if (!fireAppliedThisHit &&
            MaskManager.Instance.ElementalEffect == MaskEffectType.Fire)
        {
            fireAppliedThisHit = true;
            ApplyFireAOE(transform.position);
        }
        GetComponent<BoarChase>()?.AllowKnockback(0.08f);

        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        Flash();

        if (currentHealth <= 0)
            Die();
    }


    // Pure damage (used by poison)
    void TakePureDamage(int amount)
    {
        currentHealth -= amount;
        Flash();

        if (currentHealth <= 0)
            Die();
    }

    // ---------------- POISON ----------------

    public void ApplyPoison(int damagePerTick, float duration, float tickInterval)
    {
        if (poisonCoroutine != null)
            StopCoroutine(poisonCoroutine);

        poisonCoroutine = StartCoroutine(
            PoisonRoutine(damagePerTick, duration, tickInterval)
        );
    }

    IEnumerator PoisonRoutine(int damagePerTick, float duration, float tickInterval)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickInterval);
            TakePureDamage(damagePerTick);
            elapsed += tickInterval;
        }
    }

    // ---------------- FIRE ----------------

    void ApplyFireAOE(Vector2 center)
    {
        float radius = 2.5f;
        int damage = 1;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            BoarHealth health = hit.GetComponent<BoarHealth>();
            if (health != null && health != this)
            {
                health.TakePureDamage(damage);
            }
        }
    }

    // ---------------- VISUAL ----------------

    void Flash()
    {
        if (sr == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        sr.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    // ---------------- DEATH ----------------

    void Die()
    {
        OnDeath?.Invoke();
        DropMeat();
        Destroy(gameObject);
    }

    void DropMeat()
    {
        if (meatPickupPrefab == null || meatSprites.Length == 0)
            return;

        GameObject meat = Instantiate(meatPickupPrefab, transform.position, Quaternion.identity);

        SpriteRenderer s = meat.GetComponent<SpriteRenderer>();
        if (s != null)
            s.sprite = meatSprites[UnityEngine.Random.Range(0, meatSprites.Length)];
    }
}
