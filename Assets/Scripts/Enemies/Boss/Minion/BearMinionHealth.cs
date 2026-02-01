using UnityEngine;
using System.Collections;

public class BearMinionHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 2;
    int currentHealth;

    [Header("Hit Flash")]
    public float flashTime = 0.1f;
    public Color hitColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashCoroutine;

    public System.Action OnDeath;

    public AudioClip[] hitSounds; // assign as many as you want in Inspector
    private AudioSource audioSource;
    public AudioClip deathSound;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int amount, Vector2 hitDir)
    {
        currentHealth -= amount;
        Flash();

        // ✅ knockback hook
        var chase = GetComponent<BearMinionChase>();
        if (chase != null)
            chase.ApplyKnockback(hitDir);
        if (hitSounds != null && hitSounds.Length > 0)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (currentHealth <= 0)
            Die();
    }

    void Flash()
    {
        if (sr == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    void Die()
    {
        OnDeath?.Invoke();

        // Play death sound on a temporary object so it survives the minion being destroyed
        if (deathSound != null)
        {
            GameObject tempAudio = new GameObject("MinionDeathAudio");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = deathSound;
            aSource.Play();
            Destroy(tempAudio, deathSound.length); // destroy after sound finishes
        }

        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
