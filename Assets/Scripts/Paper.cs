using UnityEngine;

public class Paper : Interactable
{
    [SerializeField] private bool _imageOn; // Track if the image is on
    [SerializeField] private GameObject _image; // Reference to the image object (UI image)
    [SerializeField] private InventoryItem paperItem; // Reference to the ScriptableObject for the item
    private bool itemCollected = false; // Track if the item has already been collected
    private bool itemDisplayed = false; // Track if the item image is displayed

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // If the item hasn't been displayed, display the image
        if (!itemDisplayed)
        {
            _imageOn = true;
            _image.SetActive(true);
            itemDisplayed = true;  // Now the image is displayed
        }
        // If the item image is displayed, pressing interact again will add the item to inventory and hide the image
        else if (itemDisplayed && !itemCollected)
        {
            // Try to add the item to the player's inventory
            if (player.inventory.AddItem(paperItem))
            {
                Debug.Log($"{paperItem.itemName} added to inventory.");

                // Hide the image
                _imageOn = false;
                _image.SetActive(false);

                // Mark the item as collected
                itemCollected = true;

                // Now that the item is collected, destroy the paper game object
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failed to add item to inventory.");
            }
        }
    }
}
