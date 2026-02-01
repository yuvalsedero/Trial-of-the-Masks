using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public MeatUI meatUI;

    [SerializeField] private int meatCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (meatUI != null)
            meatUI.UpdateMeat(meatCount);
    }

    public int MeatCount => meatCount;

    public void AddMeat(int amount)
    {
        meatCount += Mathf.Max(0, amount);

        if (meatUI != null)
            meatUI.UpdateMeat(meatCount);
    }

    public bool SpendMeat(int amount)
    {
        if (meatCount < amount)
            return false;

        meatCount -= amount;

        if (meatUI != null)
            meatUI.UpdateMeat(meatCount);

        Debug.Log($"Meat spent. Remaining: {meatCount}");
        return true;
    }
    public bool HasEnoughMeat(int amount)
    {
        return meatCount >= amount;
    }
}
