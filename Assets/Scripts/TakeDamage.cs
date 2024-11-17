using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeDamage : MonoBehaviour
{
    public float healthCount = 3; // Keep track of how much health the player has
    public GameObject _deathTransition; // Grab the Game Over screen object
    public GameObject _basicDeathTransition;
    public GameManager gameManager;
    [SerializeField] private Animator _animator;
    public PlayerController playerController;

    // If the player collides with an enemy, reduce health by 1
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            healthCount--;
            gameManager.UpdateHealth();
            // If health reaches zero, trigger a game over
            if (healthCount <= 0 )
            {
                playerController.Dead();
                _animator.SetBool("Die", true);
                _deathTransition.SetActive(true);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crush")
        {
            healthCount = 0;
            gameManager.UpdateHealth();
            // If health reaches zero, trigger a game over
            if (healthCount <= 0)
            {
                playerController.Crush();
                _animator.SetBool("Die", true);
                _basicDeathTransition.SetActive(true);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
