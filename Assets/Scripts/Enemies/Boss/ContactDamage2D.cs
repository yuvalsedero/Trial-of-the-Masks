using UnityEngine;

public class ContactDamage2D : MonoBehaviour
{
    public int damage = 1;

    [Tooltip("Seconds between damage ticks while touching")]
    public float damageCooldown = 0.5f;

    float nextDamageTime = 0f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision);
    }

    void TryDamage(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (Time.time < nextDamageTime)
            return;

        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(damage);

        // start cooldown AFTER first hit
        nextDamageTime = Time.time + damageCooldown;
    }
}
