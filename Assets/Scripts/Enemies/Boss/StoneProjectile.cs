using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 7f;
    public float lifeTime = 5f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        Rotate(direction);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // PLAYER
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // WALL / ENVIRONMENT
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    void Rotate(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
