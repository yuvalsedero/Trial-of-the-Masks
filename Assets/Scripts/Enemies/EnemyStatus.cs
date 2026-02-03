using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    BoarHealth health;
    BoarChase chase;

    bool poisoned;
    bool slowed;

    void Awake()
    {
        health = GetComponent<BoarHealth>();
        chase = GetComponent<BoarChase>();
    }

    // 🔥 FIRE
    public void ApplyFireAOE(float radius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out BoarHealth other))
                if (other != health)
                    other.TakeDamage(damage, Vector2.zero);
        }
    }

    // ❄ ICE
    public void ApplyIceSlow(float multiplier, float duration)
    {
        if (slowed || chase == null) return;
        StartCoroutine(IceRoutine(multiplier, duration));
    }

    IEnumerator IceRoutine(float m, float d)
{
    slowed = true;

    if (m == 0f && chase != null)
    {
        chase.FreezeForSeconds(d); // this handles physics, AI, and animation freeze
    }
    else
    {
        chase.ModifySpeed(m);
    }

    yield return new WaitForSeconds(d);

    slowed = false;
}



    // ☠ POISON
    public void ApplyPoison(int damage, float duration, float tickRate)
    {
        if (poisoned) return;
        StartCoroutine(PoisonRoutine(damage, duration, tickRate));
    }

    IEnumerator PoisonRoutine(int dmg, float dur, float rate)
    {
        poisoned = true;
        float t = 0f;

        while (t < dur)
        {
            health.TakeDamage(dmg, Vector2.zero);
            yield return new WaitForSeconds(rate);
            t += rate;
        }

        poisoned = false;
    }
}
