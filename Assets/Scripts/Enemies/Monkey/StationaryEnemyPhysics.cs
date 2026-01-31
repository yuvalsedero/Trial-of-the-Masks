using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class StationaryEnemyPhysics : MonoBehaviour
{
    public float stopDelay = 0.12f;
    public float dragWhileIdle = 12f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = dragWhileIdle;
    }
    void LateUpdate()
    {
        // If something applied force this frame, kill sliding
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            StopMovement();
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
    }

    // Call this after knockback happens
    public void StopMovement()
    {
        StopAllCoroutines();
        StartCoroutine(StopRoutine());
    }

    IEnumerator StopRoutine()
    {
        yield return new WaitForSeconds(stopDelay);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
