using UnityEngine;

public class Sigil : Interactable
{
    public InventoryItem sigilItem; // Reference to the ScriptableObject for the item
    [SerializeField] private GameObject _interactIcon;
    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Check if the player's inventory can accept the item
        if (player.inventory.AddItem(sigilItem))
        {
            Debug.Log($"{sigilItem.itemName} added to inventory.");
            // Only destroy the game object after adding it to the inventory
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Failed to add item to inventory.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _interactIcon.SetActive(false);
        }
    }
}
