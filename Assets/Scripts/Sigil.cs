using UnityEngine;

public class Sigil : Interactable
{
    public InventoryItem sigilItem; // Reference to the ScriptableObject for the item
    public GameObject itemPickupUI;  // Reference to the UI GameObject


    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Check if the player's inventory can accept the item
        if (player.inventory.AddItem(sigilItem))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ShowItemUI("Sigil");
            }

            Debug.Log($"{sigilItem.itemName} added to inventory.");
            // Only destroy the game object after adding it to the inventory
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Failed to add item to inventory.");
        }
    }


}
