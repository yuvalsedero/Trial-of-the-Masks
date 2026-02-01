using UnityEngine;
using System.Collections;

public class YouDiedUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeInTime = 0.5f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
