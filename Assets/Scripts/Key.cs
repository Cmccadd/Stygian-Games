using UnityEngine;
using UnityEngine.InputSystem;

public class KeyItem : Interactable
{
    public InventoryItem keyItem;  // Reference to the key item (ScriptableObject)
    public GameObject itemPickupUI;  // Reference to the UI GameObject
    [SerializeField] private GameObject _interactIcon;
    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Add the key to the player's inventory
        if (player.inventory.AddItem(keyItem))
        {
            // Show the UI element when the item is picked up
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ShowItemUI("Key");  // Show Key UI
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
