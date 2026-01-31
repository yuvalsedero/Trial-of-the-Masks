using UnityEngine;
using System.Collections.Generic;

public class TribeChief : MonoBehaviour, IInteractable
{
    public TribeDialogData dialogData;

    [Header("Mask Pedestals")]
    public GameObject[] pedestalPrefabs;
    public Transform[] pedestalSpawnPoints;

    private readonly List<MaskPedestal> spawnedPedestals = new List<MaskPedestal>();

    enum ChiefState
    {
        BeforeGeneric,
        PedestalsSpawned,
        Completed
    }

    private ChiefState state = ChiefState.BeforeGeneric;

    public void Interact()
    {
        if (DialogManager.Instance.IsDialogOpen)
            return;

        switch (state)
        {
            case ChiefState.BeforeGeneric:
                StartConversation();
                break;

            case ChiefState.Completed:
                DialogManager.Instance.OpenDialog(
                    dialogData.completedLines,
                    null
                );
                break;
        }
    }

    // ---------- CONVERSATION FLOW ----------

    void StartConversation()
    {
        DialogManager.Instance.OpenDialog(
            dialogData.genericLines,
            AfterConversationEnds
        );

        // decide branch WHILE dialog is still open
        if (PlayerInventory.Instance.MeatCount < dialogData.meatRequired)
        {
            DialogManager.Instance.AppendLines(
                dialogData.notEnoughLines
            );
        }
        else
        {
            DialogManager.Instance.AppendLines(
                dialogData.enoughLines
            );
        }
    }

    // ---------- AFTER FINAL LINE ----------

    void AfterConversationEnds()
    {
        if (PlayerInventory.Instance.MeatCount < dialogData.meatRequired)
            return;

        SpawnPedestals();
    }

    void SpawnPedestals()
    {
        state = ChiefState.PedestalsSpawned;

        PlayerInventory.Instance.SpendMeat(dialogData.meatRequired);

        int count = Mathf.Min(pedestalPrefabs.Length, pedestalSpawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(
                pedestalPrefabs[i],
                pedestalSpawnPoints[i].position,
                Quaternion.identity
            );

            MaskPedestal pedestal = obj.GetComponent<MaskPedestal>();
            pedestal.ownerChief = this;

            spawnedPedestals.Add(pedestal);
        }
    }

    // ---------- MASK CHOSEN ----------

    public void OnMaskChosen(MaskPedestal chosen)
    {
        foreach (var p in spawnedPedestals)
        {
            if (p != null)
                Destroy(p.gameObject);
        }

        spawnedPedestals.Clear();
        state = ChiefState.Completed;
    }
}
