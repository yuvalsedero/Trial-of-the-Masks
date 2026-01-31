using UnityEngine;

public class BananaProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;

    [Header("Spin")]
    public float spinSpeed = 360f; // degrees per second

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

    void Update()
    {
        // spin around Z (2D spin)
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == ProjectileOwner.Enemy && collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        if (owner == ProjectileOwner.Player && collision.CompareTag("Enemy"))
        {
            BoarHealth health = collision.GetComponent<BoarHealth>();
            if (health != null)
                health.TakeDamage(damage, transform.right);

            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
