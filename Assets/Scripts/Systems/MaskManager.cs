using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance;

    public MaskEffectType ElementalEffect = MaskEffectType.None;
    public MaskEffectType DartEffect = MaskEffectType.None;
    public MaskEffectType StatEffect = MaskEffectType.None;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("MaskManager awake");
    }

    public void SetElemental(MaskEffectType effect)
    {
        ElementalEffect = effect;
        Debug.Log("ELEMENTAL SET TO: " + effect);
    }

    public void SetDart(MaskEffectType effect)
    {
        DartEffect = effect;
        Debug.Log("DART SET TO: " + effect);
    }

    public void ApplyStat(MaskEffectType effect)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
        Debug.Log("DART SET TO: " + effect);

        switch (effect)
        {

            case MaskEffectType.HealthUp:
                player.GetComponent<PlayerHealth>()?.IncreaseMaxHealth(1);

                break;

            case MaskEffectType.StrengthUp:
                player.GetComponent<PlayerCombatStats>()?.IncreaseDamage(1);
                break;

            case MaskEffectType.SpeedUp:
                player.GetComponent<PlayerMovement>()?.IncreaseSpeed(1.5f);
                break;
        }
    }

}
