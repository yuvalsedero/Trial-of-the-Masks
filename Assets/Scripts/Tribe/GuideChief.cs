using UnityEngine;

public class GuideChief : MonoBehaviour
{
    public TribeDialogData dialogData;

    public void Interact()
    {
        if (DialogManager.Instance.IsDialogOpen)
            return;

        DialogManager.Instance.OpenDialog(
            dialogData.genericLines,
            null
        );
    }
}
