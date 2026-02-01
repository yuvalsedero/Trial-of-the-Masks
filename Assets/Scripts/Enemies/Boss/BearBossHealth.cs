using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BearBossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 40;
    int currentHealth;

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
        Debug.Log("Boss defeated");

        Destroy(gameObject);
        // Play death sound independently
        if (deathSound != null)
        {
            GameObject tempAudio = new GameObject("BossDeathAudio");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = deathSound;
            aSource.Play();
            Destroy(tempAudio, deathSound.length); // cleanup
        }
        ScreenFade.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("EndGameScene");
        });
    }
}
