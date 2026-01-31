using UnityEngine;

public class BananaProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;

    public ProjectileOwner owner;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed, ProjectileOwner projectileOwner)
    {
        owner = projectileOwner;
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // --- ENEMY PROJECTILE ---
        if (owner == ProjectileOwner.Enemy && collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // --- PLAYER PROJECTILE ---
        if (owner == ProjectileOwner.Player && collision.CompareTag("Enemy"))
        {
            BoarHealth health = collision.GetComponent<BoarHealth>();
            if (health != null)
                health.TakeDamage(damage, transform.right);

            Destroy(gameObject);
            return;
        }

        // --- WALL / ENVIRONMENT ---
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
