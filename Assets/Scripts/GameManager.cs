using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text healthStats;
    public TakeDamage takeDamage;

    [SerializeField] private PlayerInput myPlayerInput;
    private InputAction quit;
    private InputAction restart;

    [Header("UI Settings")]
    public GameObject Sigil;
    public GameObject Key;
    public GameObject Flint;
    public GameObject ExorcismIndicator;  // New UI indicator for exorcism

    private void Start()
    {
        myPlayerInput.currentActionMap.Enable();

        quit = myPlayerInput.currentActionMap.FindAction("Quit");
        restart = myPlayerInput.currentActionMap.FindAction("Restart");

        quit.performed += Quit_performed;
        restart.performed += Restart_performed;

        // Hide all special item UI panels initially
        Sigil?.SetActive(false);
        Key?.SetActive(false);
        Flint?.SetActive(false);
        ExorcismIndicator?.SetActive(false);  // Hide exorcism indicator initially
    }

    private void Restart_performed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit_performed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    public void UpdateHealth()
    {
        if (takeDamage.healthCount > 0)
        {
            healthStats.text = "Health: " + takeDamage.healthCount;
        }
        else
        {
            healthStats.gameObject.SetActive(false);
        }
    }

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

    public void ToggleExorcismIndicator(bool show)
    {
        if (ExorcismIndicator != null)
        {
            ExorcismIndicator.SetActive(show);
        }
    }
}
