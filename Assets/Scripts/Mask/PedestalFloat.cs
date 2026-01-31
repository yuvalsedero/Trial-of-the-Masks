using UnityEngine;

public class PedestalFloat : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;


    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + Vector3.up * yOffset;

        // Optional slow spin
    }
}
