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

    AudioSource audioSource;
    [SerializeField] AudioClip[] hitSounds;
    [SerializeField] AudioClip[] deathSounds;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;

            audioSource = GetComponent<AudioSource>();
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

        if (audioSource != null)
{
    AudioClip clipToPlay = null;

    if (hitSounds != null && hitSounds.Length > 0)
    {
        clipToPlay = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
    }

    if (clipToPlay != null)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clipToPlay);
    }
}

        // ❄ ICE
        if (MaskManager.Instance.ElementalEffect == MaskEffectType.Ice)
{
    GetComponent<BoarChase>()?.FreezeForSeconds(1.5f); // full freeze including animation
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

    // ❌ Skip flashing if frozen
    BoarChase chase = GetComponent<BoarChase>();
    if (chase != null && chase.frozen)
        return;

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

    // Play death sound independently
    if (deathSounds != null && deathSounds.Length > 0)
    {
        AudioClip clipToPlay = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];

        // Create a temp GameObject for the sound
        GameObject tempAudio = new GameObject("DeathSound");
        tempAudio.transform.position = transform.position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clipToPlay;
        tempSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        tempSource.Play();

        Destroy(tempAudio, clipToPlay.length); // destroy when sound finishes
    }

    Destroy(gameObject); // destroy the boar immediately
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
