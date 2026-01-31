using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    public int bonusDamage = 0;

    public void IncreaseDamage(int amount)
    {
        bonusDamage += amount;
    }
}
