using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic Instance;

    private AudioSource audioSource;
    public float fadeDuration = 1.5f;

    private bool isFading = false;

    private void Awake()
    {
        // Singleton pattern: ensure only one music manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

            void Start()
    {
        StartCoroutine(FadeInAudio());
    }

    IEnumerator FadeInAudio()
    {
        float duration = 5f;
        float t = 0f;

        AudioListener.volume = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            AudioListener.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        AudioListener.volume = 1f;
    }


    private void Update()
    {
        // Wait for any key press to start the fade
        if (!isFading && Input.anyKeyDown)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    /// <summary>
    /// Play a clip (optional, if you want to change music)
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;

        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        Destroy(gameObject); // destroy music object
    }
}
