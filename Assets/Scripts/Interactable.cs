using UnityEngine;

public class Interactable : MonoBehaviour
{
    // This method now requires a PlayerController reference
    public virtual void InteractWith(PlayerController player)
    {
        // Override this method in specific interactable items (e.g., key, sigil)
    }
}
