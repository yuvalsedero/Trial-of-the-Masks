using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    public int maxHearts = 3;
    public int currentHearts;
    public HeartsUI heartsUI;
    public float invincibilityTime = 1f;
    public float flashTime = 0.1f;

    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Color originalColor;

    public AudioClip[] hitSounds; // add as many as you want in Inspector
    public AudioClip[] deathSounds;
    private AudioSource audioSource;

    void Awake()
    {
        currentHearts = maxHearts;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        if (heartsUI != null)
            heartsUI.UpdateHearts();
        if (sr != null)
            originalColor = sr.color;

        Debug.Log("Player hearts: " + currentHearts);
    }
    public void IncreaseMaxHealth(int amount)
    {
        maxHearts += amount;

        // always heal up when max increases
        currentHearts = maxHearts;

        if (heartsUI != null)
            heartsUI.UpdateHearts();
    }
    public void TakeDamage(int amount)
    {
        if (isInvincible)
            return;

        currentHearts -= amount;
        Flash();
        if (hitSounds != null && hitSounds.Length > 0)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (heartsUI != null)
            heartsUI.UpdateHearts();

        if (currentHearts <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }


    void Flash()
    {
        if (sr == null)
            return;

        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Player died!");

        // Play random death sound using a temporary GameObject
        float delay = 0.5f;

        if (deathSounds != null && deathSounds.Length > 0)
        {
            AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];
            GameObject tempAudio = new GameObject("TempDeathAudio");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.Play();
            delay = clip.length; // wait for sound
            Destroy(tempAudio, clip.length);
        }

        gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }

}
