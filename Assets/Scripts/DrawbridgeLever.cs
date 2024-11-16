using UnityEngine;

public class DrawbridgeLever : Interactable
{
    public GameObject /*intIcon, */lightOn, lightOff/* switchOn, switchOff*/;
    public bool toggle;
    //public AudioSource switchSound;
    [SerializeField] private AudioClip _leverpull;
    [SerializeField] private Animator _leverAnimator;
    [SerializeField] private Animator _bridgeAnimator;
    [SerializeField] private AudioSource _levelAudioSource;
    public override void InteractWith(PlayerController player)
    {
        base.InteractWith(player);
        if (toggle == true)
        {
            _leverAnimator.SetBool("Off", true);
            //switchOn.SetActive(true);
            //switchOff.SetActive(false);
            _levelAudioSource.PlayOneShot(_leverpull);
            //switchSound.Play();
            toggle = false;
            _bridgeAnimator.Play("Drawbridge");
        }
        
    }
}
