using UnityEngine;

public class KeyItem : Interactable
{
    public InventoryItem keyItem;  // Reference to the key item (ScriptableObject)

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Add the key to the player's inventory
        player.inventory.AddItem(keyItem);

        // Optionally, display a message or sound when the key is picked up
        Debug.Log("Key picked up!");

        // Destroy the key object in the world
        Destroy(gameObject);
    }
}
