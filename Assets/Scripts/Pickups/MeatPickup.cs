using UnityEngine;

public class MeatPickup : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory PlayerInventory = other.GetComponent<PlayerInventory>();
        if (PlayerInventory != null)
        {
            PlayerInventory.Instance.AddMeat(1);

        }

        Destroy(gameObject);
    }
}
