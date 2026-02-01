using UnityEngine;
using UnityEngine.UI;

public class MaskPiecesUI : MonoBehaviour
{
    public Image[] pieces; // size = 3

    public Color emptyColor = new Color(1f, 1f, 1f, 0.25f);
    public Color filledColor = Color.white;

    void Start()
    {
        // init from current state
        UpdateUI(MaskManager.Instance.maskPiecesCollected);

        // listen to changes
        MaskManager.Instance.OnMaskPieceCollected += UpdateUI;
    }

    void OnDestroy()
    {
        if (MaskManager.Instance != null)
            MaskManager.Instance.OnMaskPieceCollected -= UpdateUI;
    }

    void UpdateUI(int count)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].color = i < count ? filledColor : emptyColor;
        }
    }
}
