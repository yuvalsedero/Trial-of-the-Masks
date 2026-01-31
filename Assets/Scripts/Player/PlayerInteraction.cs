using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable nearbyInteractable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            nearbyInteractable = interactable;
            Debug.Log("[PlayerInteraction] Entered range of interactable");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (nearbyInteractable == interactable)
            {
                Debug.Log("[PlayerInteraction] Left range of interactable");
                nearbyInteractable = null;
            }
        }
    }

    void Update()
    {
        if (DialogManager.Instance != null &&
            DialogManager.Instance.IsDialogOpen)
            return;

        if (nearbyInteractable == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[PlayerInteraction] E pressed");
            nearbyInteractable.Interact();
        }
    }
}
