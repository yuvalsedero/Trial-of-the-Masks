using UnityEngine;
using System.Collections;

public class FlashingText : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float flashSpeed = 1.2f;

    Coroutine flashRoutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFlashing()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        while (true)
        {
            // fade in
            yield return FadeTo(1f);
            // fade out
            yield return FadeTo(0f);
        }
    }

    IEnumerator FadeTo(float target)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha,
                target,
                Time.deltaTime * flashSpeed
            );
            yield return null;
        }
    }
}
