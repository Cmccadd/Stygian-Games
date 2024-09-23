using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _player;

    /// <summary>
    /// Used to make the camera stay following the player while they move, using the offset to stay the same distance 
    /// away
    /// </summary>
    void LateUpdate() //Fraction of a second after udate, makes vehicle less jittery
    {
        // Makes the camera follow player as they travel
        transform.position = new Vector3 (_player.position.x, 3.4f, -13.3f);
    }
}
