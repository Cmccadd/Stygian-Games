using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>(); // The list of items in the inventory

    // Add an item to the inventory
    public void AddItem(InventoryItem item)
    {
        if (item != null)
        {
            items.Add(item);
            Debug.Log($"Picked up: {item.itemName}");
        }
    }

    // Remove an item from the inventory
    public void RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Removed: {item.itemName}");
        }
    }

    // Check if the inventory contains a specific item by name
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
}
