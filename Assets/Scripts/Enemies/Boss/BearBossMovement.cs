using UnityEngine;

public class BearBossMovement : MonoBehaviour
{
    public float moveSpeed = 1.2f;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveTowards(Vector2 target)
    {
        Vector2 dir = (target - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // face direction
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
