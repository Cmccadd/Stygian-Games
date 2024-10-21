using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Flint : Interactable
{
    public InventoryItem flintItem;  // Reference to the key item (ScriptableObject)
    public GameObject itemPickupUI;  // Reference to the UI GameObject

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Add the key to the player's inventory
        if (player.inventory.AddItem(flintItem))
        {
            // Show the UI element when the item is picked up
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ShowItemUI("Flint");  // Show Key UI
            }

            // Optionally, display a message or sound when the key is picked up
            Debug.Log("Key picked up!");

            // Destroy the key object in the world
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Failed to add key to inventory.");
        }
    }
}
