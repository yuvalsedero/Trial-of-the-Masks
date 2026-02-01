using UnityEngine;

public class ChiefDialogueSound : MonoBehaviour
{
    [Header("Voice Settings")]
    public AudioSource voiceSource;    // NPC's own AudioSource
    public AudioClip[] lineSounds;    // NPC's own voice clips

    // Call this when player interacts with the NPC
    public void AssignVoiceToDialogManager()
    {
        if (DialogManager.Instance == null)
            return;

        // Assign this NPC's AudioSource and clips to the DialogManager
        DialogManager.Instance.voiceSource = voiceSource;
        DialogManager.Instance.lineSounds = lineSounds;
    }
}
