using UnityEngine;
using System.Collections;

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
}
