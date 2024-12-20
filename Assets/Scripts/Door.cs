using UnityEngine;

public class Door : Interactable
{
    public string requiredKeyName; // The name of the key item required to unlock the door
    public bool isUnlocked = false; // Whether the door is unlocked
    public Animator doorAnimator; // Optional: Animator for door open/close animation
   // [SerializeField] private GameObject _interactIcon;
    [SerializeField] private bool _imageOn; // Track if the image is on
    [SerializeField] private AudioClip _doorOpen;
    [SerializeField] private AudioSource _myAudioSource;

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
            // Unlock the door
            UnlockDoor();

            // Remove the key from the inventory
            player.inventory.UseItem(requiredKeyName);

            // Hide the UI for the key
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.HideItemUI("Key");  // Hide Key UI when it's used
            }
        }
        else
        {

            Debug.Log("Door is locked. You need the key.");
        }
    }

    private void UnlockDoor()
    {
        _myAudioSource.PlayOneShot(_doorOpen);
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        _interactIcon.SetActive(true);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        _interactIcon.SetActive(false);
    //    }
    //}
}
