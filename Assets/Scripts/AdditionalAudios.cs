using UnityEngine;

public class AdditionalAudios : MonoBehaviour
{
    [SerializeField] private AudioClip _cough;
    [SerializeField] private AudioClip _hymns;
    [SerializeField] private AudioClip _gasp;
    [SerializeField] private AudioClip _doorCreak;
    [SerializeField] private AudioClip _thud;
    [SerializeField] private AudioSource _myAudioSource;
    public void Cough()
    {
        _myAudioSource.PlayOneShot(_cough);
    }

    public void Hymns()
    {
        _myAudioSource.PlayOneShot(_hymns);
    }

    public void Gasp()
    {
        _myAudioSource.PlayOneShot(_gasp);
    }

    public void DoorCreak()
    {
        _myAudioSource.PlayOneShot(_doorCreak);
    }

    public void Thud()
    {
        _myAudioSource.PlayOneShot(_thud);
    }
}
