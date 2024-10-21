using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text healthStats; // Grab the text object
    public TakeDamage takeDamage; // Grab the script for taking damage

    [SerializeField] private PlayerInput myPlayerInput;
    private InputAction quit;
    private InputAction restart;

    [Header("UI Settings")]
    public GameObject Sigil;  // UI for Sigil
    public GameObject Key;    // UI for Key
    public GameObject Flint;  // UI for Flint

    private void Start()
    {
        myPlayerInput.currentActionMap.Enable();

        quit = myPlayerInput.currentActionMap.FindAction("Quit");
        restart = myPlayerInput.currentActionMap.FindAction("Restart");

        quit.performed += Quit_performed;
        restart.performed += Restart_performed;

        // Initially hide the special item UI panels
        Sigil?.SetActive(false);
        Key?.SetActive(false);
        Flint?.SetActive(false);
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

    // Show UI for a specific item based on its name
    public void ShowItemUI(string itemName)
    {
        if (itemName == "Sigil" && Sigil != null)
        {
            Sigil.SetActive(true);
        }
        else if (itemName == "Key" && Key != null)
        {
            Key.SetActive(true);
        }
        else if (itemName == "Flint" && Flint != null)
        {
            Flint.SetActive(true);
        }
    }

    // Hide UI for a specific item based on its name
    public void HideItemUI(string itemName)
    {
        if (itemName == "Sigil" && Sigil != null)
        {
            Sigil.SetActive(false);
        }
        else if (itemName == "Key" && Key != null)
        {
            Key.SetActive(false);
        }
        else if (itemName == "Flint" && Flint != null)
        {
            Flint.SetActive(false);
        }
    }
}
