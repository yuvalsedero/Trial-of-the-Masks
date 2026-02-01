using TMPro;
using UnityEngine;

public class ChiefIndicator : MonoBehaviour
{
    public TribeChief chief;

    [Header("Visuals")]
    public GameObject exclamationMark;
    public GameObject meatGroup;
    public TextMeshPro meatText;

    void Update()
    {
        if (chief == null || PlayerInventory.Instance == null)
            return;

        // 1️⃣ If pedestals are spawned OR completed → show nothing
        if (chief.IsWaitingForChoice || chief.IsCompleted)
        {
            exclamationMark.SetActive(false);
            meatGroup.SetActive(false);
            return;
        }

        // 2️⃣ Only BEFORE conversation starts
        if (!chief.CanStartConversation)
        {
            exclamationMark.SetActive(false);
            meatGroup.SetActive(false);
            return;
        }

        int cost = chief.dialogData.meatCost;
        int playerMeat = PlayerInventory.Instance.MeatCount;

        bool canTalk = playerMeat >= cost;

        exclamationMark.SetActive(canTalk);
        meatGroup.SetActive(!canTalk);

        if (!canTalk)
        {
            meatText.text = cost.ToString();
        }
    }
}
