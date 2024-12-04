using UnityEngine;

public class WitchCutsceneSwaps : MonoBehaviour
{
    [SerializeField] private GameObject _witch;
    [SerializeField] private GameObject _normalTorch;
    [SerializeField] private GameObject _fireBurst;
    [SerializeField] private GameObject _blueTorch;
    [SerializeField] private GameObject _cutsceneCam;
    [SerializeField] private GameObject _enemyIndicator;
    [SerializeField] private AudioSource _myAudioSource;
    [SerializeField] private AudioClip _fire;
    public void TurnOnOffStuff()
    {
        _witch.SetActive(true);
        _normalTorch.SetActive(false);
        _blueTorch.SetActive(true);
        _enemyIndicator.SetActive(true);
    }

    public void TurnOffCamera()
    {
        _cutsceneCam.SetActive(false);
    }

    public void TurnOnFire()
    {
        _fireBurst.SetActive(true);
        _myAudioSource.PlayOneShot(_fire);
    }

    public void TurnOffFire()
    {
        _fireBurst.SetActive(false);
    }
}
