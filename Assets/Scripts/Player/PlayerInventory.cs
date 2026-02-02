using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public MeatUI meatUI;

    [Header("Audio")]
    public AudioClip[] meatPickupSounds;
    private AudioSource audioSource;

    [SerializeField] private int meatCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (meatUI != null)
            meatUI.UpdateMeat(meatCount);
    }

    public int MeatCount => meatCount;

    public void AddMeat(int amount)
    {
        if (amount <= 0)
            return;

        meatCount += amount;

        if (meatUI != null)
            meatUI.UpdateMeat(meatCount);

        PlayMeatPickupSound();
    }

    void PlayMeatPickupSound()
    {
        if (meatPickupSounds == null || meatPickupSounds.Length == 0)
            return;

        AudioClip clip = meatPickupSounds[Random.Range(0, meatPickupSounds.Length)];
        audioSource.PlayOneShot(clip);
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
