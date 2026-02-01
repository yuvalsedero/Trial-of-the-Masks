using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BearBossHealth : MonoBehaviour, IDamageable
{
    bool isDead = false;

    public int maxHealth = 40;
    public int currentHealth;

    [Header("Hit Flash")]
    public float flashTime = 0.1f;
    public Color hitColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashCoroutine;

    public System.Action OnPhaseTwo;
    bool phaseTwoTriggered = false;

    [Header("Hit Sounds")]
    public AudioClip[] hitSounds;
    private AudioSource audioSource;
    private AudioClip lastHitClip; // Prevent same clip twice in a row

    [Header("Death Sound")]
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

        audioSource.playOnAwake = false;
    }

    public void TakeDamage(int amount, Vector2 hitDir)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        Flash();
        PlayHitSoundSteal();

        if (!phaseTwoTriggered && currentHealth <= maxHealth / 2)
        {
            phaseTwoTriggered = true;
            OnPhaseTwo?.Invoke();
        }

        if (currentHealth <= 0)
            Die();
    }

    void PlayHitSoundSteal()
    {
        if (hitSounds == null || hitSounds.Length == 0)
            return;

        AudioClip clip;
        if (hitSounds.Length == 1)
        {
            clip = hitSounds[0];
        }
        else
        {
            // Pick a new clip that isn’t the same as last one
            do
            {
                clip = hitSounds[Random.Range(0, hitSounds.Length)];
            } while (clip == lastHitClip);
        }

        lastHitClip = clip;

        // **Steal behavior:** stop previous hit sound if still playing
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();
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

    public Transform GetTransform()
    {
        return transform;
    }
    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Boss defeated");

        // 🔒 FREEZE PHYSICS COMPLETELY
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // 🚨 THIS STOPS ALL PHYSICS
        }

        // 🔒 Disable ALL colliders
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        // 🔒 Disable ALL scripts except this one
        foreach (MonoBehaviour mb in GetComponents<MonoBehaviour>())
        {
            if (mb != this)
                mb.enabled = false;
        }

        // 👻 Hide visually
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // 🔊 Play death sound
        if (deathSound != null)
        {
            GameObject tempAudio = new GameObject("BossDeathAudio");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = deathSound;
            aSource.Play();
            Destroy(tempAudio, deathSound.length);
        }

        // 🎬 Transition
        ScreenFade.Instance.FadeOut(() =>
        {
            GameManager.Instance.LoadSceneAfterDelay("EndGameScene", 0f);
        });
    }


}
