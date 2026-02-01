using UnityEngine;
using System.Collections;

public class HitFlashFX : MonoBehaviour
{
    public float duration = 0.15f;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(Color color)
    {
        sr.color = color;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color start = sr.color;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            sr.color = new Color(start.r, start.g, start.b, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
