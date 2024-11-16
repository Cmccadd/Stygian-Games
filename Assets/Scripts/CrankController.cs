using System.Collections;
using UnityEngine;

public class CrankController : Interactable
{
    public GameObject Gate;
    public GameObject CrankSFX;
    //public GameObject _interactIcon;
    public Rigidbody Rigidbody;
    public bool PlayerInteracting;
    [SerializeField] private int _crankCooldown = 2;
    [SerializeField] private int _crankRate = 5;
    [SerializeField] Animator _crankAnimator;

    // InteractWith now takes a PlayerController parameter to match the base class
    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);
        PlayerInteracting = true;
        StartCoroutine(Cranking());
        _crankAnimator.SetBool("Cranking", true);
        CrankSFX.SetActive(true);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        _interactIcon.SetActive(true);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //_interactIcon.SetActive(false);
            PlayerInteracting = false;
            Rigidbody.velocity = new Vector3(0, -_crankRate / 2, 0); // Stop or reverse the crank
            _crankAnimator.SetBool("Cranking", false);
            CrankSFX.SetActive(false);
        }
    }

    private IEnumerator Cranking()
    {
        // Simulate crank operation with a cooldown
        Rigidbody.velocity = new Vector3(0, _crankRate, 0);
        yield return new WaitForSeconds(_crankCooldown);
        //Rigidbody.velocity = Vector3.zero; // Stop the crank after cooldown
    }
}
