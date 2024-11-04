using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _teleportLocation;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = _teleportLocation.transform.position;
        }
    }
}