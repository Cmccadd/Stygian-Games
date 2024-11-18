using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalAudios : MonoBehaviour
{
    [SerializeField] private AudioClip _cough;
    [SerializeField] private AudioSource _myAudioSource;
    public void Cough()
    {
        _myAudioSource.PlayOneShot(_cough);
    }
}
