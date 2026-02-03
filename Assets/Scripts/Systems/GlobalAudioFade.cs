using UnityEngine;
using System.Collections;

public class GlobalAudioFade : MonoBehaviour
{
    public static GlobalAudioFade Instance;

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

    public void FadeOutAllAudio(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAudio(1f, 0f, duration));
    }

    public void FadeInAllAudio(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAudio(AudioListener.volume, 1f, duration));
    }

    IEnumerator FadeAudio(float from, float to, float duration)
    {
        float t = 0f;
        AudioListener.volume = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            AudioListener.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        AudioListener.volume = to;
    }
}
