using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHearts = 3;
    public int currentHearts;
    public HeartsUI heartsUI;
    public float invincibilityTime = 1f;
    public float flashTime = 0.1f;

    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Color originalColor;
    void Awake()
    {
        currentHearts = maxHearts;
    }
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (heartsUI != null)
            heartsUI.UpdateHearts();
        if (sr != null)
            originalColor = sr.color;

        Debug.Log("Player hearts: " + currentHearts);
    }
    public void IncreaseMaxHealth(int amount)
    {
        maxHearts += amount;

        // always heal up when max increases
        currentHearts = maxHearts;

        if (heartsUI != null)
            heartsUI.UpdateHearts();
    }
    public void TakeDamage(int amount)
    {
        if (isInvincible)
            return;

        currentHearts -= amount;
        Flash();

        if (heartsUI != null)
            heartsUI.UpdateHearts();

        if (currentHearts <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }


    void Flash()
    {
        if (sr == null)
            return;

        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
}
