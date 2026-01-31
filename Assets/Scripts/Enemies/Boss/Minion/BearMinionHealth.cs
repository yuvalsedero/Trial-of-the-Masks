using UnityEngine;
using System.Collections;

public class BearMinionHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 2;
    int currentHealth;

    [Header("Hit Flash")]
    public float flashTime = 0.1f;
    public Color hitColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashCoroutine;

    public System.Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(int amount, Vector2 hitDir)
    {
        currentHealth -= amount;
        Flash();

        if (currentHealth <= 0)
            Die();
    }

    void Flash()
    {
        if (sr == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
