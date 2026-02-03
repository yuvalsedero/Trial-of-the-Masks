using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;

    public CanvasGroup fadeGroup;
    public float fadeDuration = 3f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, 1f, onComplete));
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, 0f, onComplete));
    }

    IEnumerator Fade(float from, float to, System.Action onComplete)
    {
        float t = 0f;
        fadeGroup.alpha = from;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = to;
        onComplete?.Invoke();
    }
    void OnEnable()
{
    // Subscribe to the sceneLoaded event
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    // Unsubscribe to prevent memory leaks
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // Reset the alpha so the player can see the new scene
    if (fadeGroup != null)
    {
        fadeGroup.alpha = 0f; 
        // Or call FadeIn() here if you want a smooth transition every time
    }
}
    
}
