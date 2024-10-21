using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>(); // List to hold inventory items
    public int maxInventorySize = 10;  // Optional limit for inventory size

    // Add item to the inventory and return true if successful
    public bool AddItem(InventoryItem item)
    {
        if (items.Count < maxInventorySize)
        {
            items.Add(item);
            Debug.Log($"{item.itemName} added to inventory.");

            // Notify GameManager to show the UI for the added item
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ShowItemUI(item.itemName); // Show the specific item UI
            }

            return true;
        }
        else
        {
            Debug.Log("Inventory is full!");
            return false;
        }
    }

    // Check if the inventory contains an item by name
    public bool HasItem(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }
        return false;
    }

    // Use an item and remove it from the inventory
    public void UseItem(string itemName)
    {
        InventoryItem itemToUse = null;

        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                itemToUse = item;
                break;
            }
        }

        if (itemToUse != null)
        {
            items.Remove(itemToUse);
            Debug.Log($"{itemName} used and removed from inventory.");

            // Notify GameManager to turn off the UI for the used item
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.HideItemUI(itemName); // Hide the specific item UI when the item is used
            }
        }
        else
        {
            Debug.LogError($"{itemName} not found in inventory.");
        }
    }
}
