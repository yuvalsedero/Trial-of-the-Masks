using UnityEngine;
using System.Collections.Generic;

public class TribeChief : MonoBehaviour, IInteractable
{
    public TribeDialogData dialogData;

    [Header("Mask Pedestals")]
    public GameObject[] pedestalPrefabs;
    public Transform[] pedestalSpawnPoints;

    private readonly List<MaskPedestal> spawnedPedestals = new();

    enum ChiefState
    {
        NotStarted,
        PedestalsSpawned,
        Completed
    }

    private ChiefState state = ChiefState.NotStarted;

    public void Interact()
    {
        if (DialogManager.Instance.IsDialogOpen)
            return;

        switch (state)
        {
            case ChiefState.NotStarted:
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

    void StartConversation()
    {
        DialogManager.Instance.OpenDialog(
            dialogData.genericLines,
            SpawnPedestals
        );
    }

    void SpawnPedestals()
    {
        if (state != ChiefState.NotStarted)
            return;

        state = ChiefState.PedestalsSpawned;

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
