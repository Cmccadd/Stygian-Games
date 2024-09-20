
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public CheckpointManager CM; //Grabs the game manager

    /// <summary>
    /// Grabs the GameManager script at the start.
    /// </summary>
    void Start()
    {
        CM = FindObjectOfType<CheckpointManager>();
    }

    /// <summary>
    /// Checks if the player collides with this object, and if so, updates the GameManager.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // Checks if this object collides with an object with the "Player" tag
        if (other.gameObject.tag == "Player")
        {
            // Sets the GameManager's checkpoint system to this current checkpoint
            CM.LastCheckPointPos = transform.position;
            // Removes the checkpoint, making it so the player can't accidentally go back to an older checkpoint
            Destroy(gameObject);
        }
    }
}
