using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrankController : Interactable
{
    public GameObject Gate;
    public Rigidbody Rigidbody;
    public bool PlayerInteracting;
    [SerializeField] private int _crankCooldown;
    [SerializeField] private int _crankRate;

    public override void InteractWith()
    {
        base.InteractWith();
        PlayerInteracting = true;
        StartCoroutine(Cranking());
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInteracting = false;
        Rigidbody.velocity = new Vector3(0, -_crankRate/2, 0);

    }

    public IEnumerator Cranking()
    {
        Rigidbody.velocity = new Vector3(0, _crankRate, 0);
        yield return new WaitForSeconds(_crankCooldown);
    }
}
