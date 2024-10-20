using UnityEngine;

public class ToggleTorch : Interactable
{
    //public InventoryItem paperItem; // Reference to the ScriptableObject for the item
    [SerializeField] private bool _imageOn;
    [SerializeField] private GameObject _torch;
    [SerializeField] private GameObject _interactIcon;

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);
        _imageOn = true;
        _torch.SetActive(true);
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
