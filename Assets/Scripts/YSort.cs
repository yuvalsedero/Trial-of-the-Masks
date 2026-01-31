using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    public Transform sortPoint;   // where "feet" are
    public int offset = 0;
    public int multiplier = 100;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // fallback if not assigned
        if (sortPoint == null)
            sortPoint = transform;
    }

    void LateUpdate()
    {
        sr.sortingOrder = -(int)(sortPoint.position.y * multiplier) + offset;
    }
}
