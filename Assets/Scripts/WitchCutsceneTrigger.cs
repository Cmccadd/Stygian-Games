using UnityEngine;

public class WitchCutsceneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _witchCutsceneCam;
    [SerializeField] private GameObject _witchCutsceneTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _witchCutsceneCam.SetActive(true);
            _witchCutsceneTrigger.SetActive(false);
        }
    }
}
