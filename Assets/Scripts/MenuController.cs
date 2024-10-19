using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    [SerializeField] private GameObject Cutscene;

    /// <summary>
    /// Loads the next scene in the build index.
    /// </summary>
    public void StartCutscene()
    {
        Cutscene.SetActive(true);
    }

    /// <summary>
    /// Loads the next scene
    /// </summary>
    private void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    /// <param name="context"></param>
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void Menu()
    {
        // Loads the first scene, which is the main menu
        SceneManager.LoadScene(0);
    }
}