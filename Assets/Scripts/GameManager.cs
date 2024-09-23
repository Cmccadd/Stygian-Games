using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text healthStats; // Grab the text object

    public TakeDamage takeDamage; // Grab the script for taking damage

    // Keep track of current player health state

    [SerializeField] private PlayerInput myPlayerInput;

    private InputAction quit;
    private InputAction restart;




    private void Start()
    {
        myPlayerInput.currentActionMap.Enable();

        quit = myPlayerInput.currentActionMap.FindAction("Quit");
        restart = myPlayerInput.currentActionMap.FindAction("Restart");


        quit.performed += Quit_performed;
        restart.performed += Restart_performed;


    }

    private void Restart_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Restart Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    private void Quit_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void UpdateHealth()
    {
        if (takeDamage.healthCount > 0) // Only update health if health isn't at 0
        {
            healthStats.text = "Health: " + takeDamage.healthCount; // Display the health stats
        }
        else
        {
            healthStats.gameObject.SetActive(false); // Disable the health text on the game over screen
        }
    }
}
