using UnityEngine;
using UnityEngine.UI;

public class MaskPiecesUI : MonoBehaviour
{
    [Header("Mask Piece Images (order matters)")]
    public Image[] maskPieces; // size = 3

    void Start()
    {
        // hide all at start
        foreach (var img in maskPieces)
            img.enabled = false;

        // sync immediately (safe if loading mid-game)
        UpdateUI(MaskManager.Instance.maskPiecesCollected);

        MaskManager.Instance.OnMaskPieceCollected += UpdateUI;
    }

    void OnDestroy()
    {
        if (MaskManager.Instance != null)
            MaskManager.Instance.OnMaskPieceCollected -= UpdateUI;
    }

    void UpdateUI(int count)
    {
        for (int i = 0; i < maskPieces.Length; i++)
        {
            maskPieces[i].enabled = i < count;
        }
    }
}
