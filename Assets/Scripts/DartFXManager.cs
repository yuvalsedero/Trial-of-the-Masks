using UnityEngine;

public class DartFXManager : MonoBehaviour
{
    public static DartFXManager Instance;

    [Header("FX Prefabs")]
    public GameObject fireFXPrefab;
    public GameObject iceFXPrefab;
    public GameObject poisonFXPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Spawn elemental FX at a hit point
    /// </summary>
    public void SpawnFX(Vector3 position, MaskEffectType effect, float scale = 1f)
    {
        GameObject prefab = effect switch
        {
            MaskEffectType.Fire => fireFXPrefab,
            MaskEffectType.Ice => iceFXPrefab,
            MaskEffectType.Poison => poisonFXPrefab,
            _ => null
        };

        if (prefab == null) return;

        GameObject fx = Instantiate(prefab, position, Quaternion.identity);
        fx.transform.localScale = Vector3.one * scale;

        if (fx.GetComponent<FXSelfDestruct>() == null)
            fx.AddComponent<FXSelfDestruct>();
    }
}
