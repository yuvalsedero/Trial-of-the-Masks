using UnityEngine;
using System.Collections;

public class BearBossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 40;
    int currentHealth;

    [Header("Hit Flash")]
    public float flashTime = 0.1f;
    public Color hitColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashCoroutine;

    public System.Action OnPhaseTwo;
    bool phaseTwoTriggered = false;

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

        if (!phaseTwoTriggered && currentHealth <= maxHealth / 2)
        {
            phaseTwoTriggered = true;
            OnPhaseTwo?.Invoke();
        }

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

    public Transform GetTransform()
    {
        return transform;
    }

    void Die()
    {
        Debug.Log("Boss defeated");
        Destroy(gameObject);
    }
}
