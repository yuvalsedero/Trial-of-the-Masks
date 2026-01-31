using UnityEngine;

public enum MaskCategory
{
    Elemental,
    Dart,
    Stat
}

public class MaskPedestal : MonoBehaviour
{
    public MaskEffectType effectType;
    public MaskCategory category;
    public TribeChief ownerChief;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        switch (category)
        {
            case MaskCategory.Elemental:
                MaskManager.Instance.SetElemental(effectType);
                break;

            case MaskCategory.Dart:
                MaskManager.Instance.SetDart(effectType);
                break;

            case MaskCategory.Stat:
                MaskManager.Instance.ApplyStat(effectType);
                break;
        }

        ownerChief.OnMaskChosen(this);
        Destroy(gameObject);
    }
}
