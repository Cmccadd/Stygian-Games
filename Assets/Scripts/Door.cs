using UnityEngine;

public class Door : Interactable
{
    public string requiredKeyName; // The name of the key item required to unlock the door
    public bool isUnlocked = false; // Whether the door is unlocked
    public Animator doorAnimator; // Optional: Animator for door open/close animation

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);

        if (isUnlocked)
        {
            Debug.Log("Door is already unlocked.");
            OpenDoor();
        }
        else if (player.inventory.HasItem(requiredKeyName))
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("Door is locked. You need the key.");
        }
    }

    private void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("Door unlocked!");
        OpenDoor();
    }

    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");  // Play door opening animation if using an Animator
        }
        else
        {
            // If no animation, you can move or disable the door object to simulate it opening
            gameObject.SetActive(false);
        }
    }
}
