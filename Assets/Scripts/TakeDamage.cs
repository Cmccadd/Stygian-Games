using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeDamage : MonoBehaviour
{
    public float healthCount = 3; // Keep track of how much health the player has
    public GameObject gameOver; // Grab the Game Over screen object

    // If the player collides with an enemy, reduce health by 1
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Enemy")
        {
            healthCount--;

            // If health reaches zero, trigger a game over
            if (healthCount <= 0 )
            {
                gameOver.SetActive(true);
                SceneManager.LoadScene(0);
            }
        }
    }
}
