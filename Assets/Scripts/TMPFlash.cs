using UnityEngine;
using TMPro;

public class TMPFlash : MonoBehaviour
{
    public float flashSpeed = 2f;   // flashes per second
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    TextMeshProUGUI tmp;
    Color baseColor;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        baseColor = tmp.color;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.unscaledTime * flashSpeed, 1f);
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);

        tmp.color = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            a
        );
    }
}
