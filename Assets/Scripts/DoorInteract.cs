using UnityEngine;

public class DoorInteract : Interactable
{
    [SerializeField] private GameObject _fadeOut;
    //[SerializeField] private GameObject _interactIcon;

    public override void InteractWith(PlayerController player)
    {
        _fadeOut.SetActive(true);
        base.InteractWith(player);
        
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
