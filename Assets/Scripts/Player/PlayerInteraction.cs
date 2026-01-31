using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private TribeChief nearbyChief;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out TribeChief chief))
        {
            nearbyChief = chief;
            Debug.Log($"[PlayerInteraction] Entered range of TribeChief: {chief.dialogData.tribeType}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out TribeChief chief))
        {
            if (nearbyChief == chief)
            {
                Debug.Log($"[PlayerInteraction] Left range of TribeChief: {chief.dialogData.tribeType}");
                nearbyChief = null;
            }
        }
    }

    void Update()
    {
        if (DialogManager.Instance != null &&
        DialogManager.Instance.IsDialogOpen)
            return;
        if (nearbyChief == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[PlayerInteraction] E pressed near TribeChief");
            nearbyChief.Interact();
        }
    }
}
