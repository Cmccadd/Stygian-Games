using UnityEngine;

public class LeverController : Interactable
{
    public GameObject /*intIcon, */lightOn, lightOff/* switchOn, switchOff*/;
    public bool toggle;
    //public AudioSource switchSound;
    [SerializeField] private AudioClip _leverpull;
    [SerializeField] private Animator _leverAnimator;
    [SerializeField] private AudioSource _levelAudioSource;

    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);
        if (toggle == true)
        {
            lightOn.SetActive(true);
            lightOff.SetActive(false);
            _leverAnimator.SetBool("Off", true);
            //switchOn.SetActive(true);
            //switchOff.SetActive(false);
            _levelAudioSource.PlayOneShot(_leverpull);
            //switchSound.Play();
            toggle = false;
        }
        else if (toggle == false)
        {
            lightOn.SetActive(false);
            lightOff.SetActive(true);
            _leverAnimator.SetBool("Off", false);
            //switchOn.SetActive(false);
            //switchOff.SetActive(true);
            _levelAudioSource.PlayOneShot(_leverpull);
            //switchSound.Play();
            toggle = true;
        }
    }
    
    //void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("MainCamera"))
    //    {
    //        intIcon.SetActive(false);
    //    }
    //}
}
