using UnityEngine;

public class Paper : Interactable
{
    //public InventoryItem paperItem; // Reference to the ScriptableObject for the item
    [SerializeField] private bool _imageOn;
    [SerializeField] private GameObject _image;

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        // Check if the player's inventory can accept the item
        //if (player.inventory.AddItem(paperItem))
        //{
            //Debug.Log($"{paperItem.itemName} added to inventory.");
            // Only destroy the game object after adding it to the inventory
            if (_imageOn == false)
            {
                _imageOn = true;
                _image.SetActive(true);
            }
            else if (_imageOn == true)
            {
                _imageOn = false;
                _image.SetActive(false);
            }
            //_imageOn = true;
            //_image.SetActive(true);
            //Destroy(gameObject);
        //}
        //else
        //{
          //  Debug.Log("Failed to add item to inventory.");
        //}
    }
}
