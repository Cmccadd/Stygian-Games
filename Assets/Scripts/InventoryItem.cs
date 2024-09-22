using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName; // Name of the item
    public Sprite itemIcon; // Icon to display in the inventory UI (optional)
}
