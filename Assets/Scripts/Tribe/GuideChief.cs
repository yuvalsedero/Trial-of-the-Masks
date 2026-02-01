using UnityEngine;

public class GuideChief : MonoBehaviour, IInteractable
{
    public TribeDialogData dialogData;

    [Header("Voice")]
    public DialogVoiceSet voiceSet;

    public void Interact()
    {
        if (DialogManager.Instance.IsDialogOpen)
            return;

        // 🔊 tell dialog manager which voice to use
        DialogManager.Instance.SetVoice(voiceSet);

        DialogManager.Instance.OpenDialog(
            dialogData.genericLines,
            null
        );
    }
}
