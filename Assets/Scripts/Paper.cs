using UnityEngine;

public class Paper : Interactable
{
    [SerializeField] private bool _imageOn; // Track if the image is on
    [SerializeField] private GameObject _image; // Reference to the image object (UI image)
    private bool itemDisplayed = false; // Track if the item image is displayed
    //[SerializeField] private GameObject _interactIcon;
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
        // If the item image is already displayed, pressing interact again will hide the image
        else if (itemDisplayed)
        {
            _imageOn = false;
            _image.SetActive(false);
            itemDisplayed = false;  // Reset, allowing future interactions
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        _interactIcon.SetActive(true);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //_interactIcon.SetActive(false);
            if (itemDisplayed)
            {
                _imageOn = false;
                _image.SetActive(false);
                itemDisplayed = false;  // Reset, allowing future interactions
            }
        }
    }
}
