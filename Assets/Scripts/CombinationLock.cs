using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    [SerializeField] private GameObject _torch1;
    [SerializeField] private GameObject _torch2;
    [SerializeField] private GameObject _torch3;
    [SerializeField] private GameObject _torch4;
    [SerializeField] private GameObject _door;
    [SerializeField] private bool _t1On;
    [SerializeField] private bool _t2On;
    [SerializeField] private bool _t3On;
    [SerializeField] private bool _t4On;
    [SerializeField] private AudioClip _doorOpen;
    [SerializeField] private AudioSource _myAudioSource;

    // Update is called once per frame
    void Update()
    {
        if (_torch1.activeInHierarchy == true)
        {
            _t1On = true;
        }
        if (_torch2.activeInHierarchy == true)
        {
            Fail();
        }
        if (_torch3.activeInHierarchy == true)
        {
            if (_t1On)
            {
                _t3On = true;
            }
            else
            {
                Fail();
            }
        }
        if (_torch4.activeInHierarchy == true)
        {
            if (_t1On && _t3On)
            {
                _t4On = true;
            }
            else
            {
                Fail();
            }
        }
        if (_t4On)
        {
            _door.SetActive(false);
            _myAudioSource.PlayOneShot(_doorOpen);
        }
        
    }
    
    private void Fail()
    {
        _torch1.SetActive(false);
        _torch2.SetActive(false);
        _torch3.SetActive(false);
        _torch4.SetActive(false);
    }
}
