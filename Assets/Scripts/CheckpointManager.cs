
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager instance;
    // This need to be public so they can be accessed by the Checkpoint Script
    [SerializeField] private Vector3 lastCheckPointPos;

    public Vector3 LastCheckPointPos { get => lastCheckPointPos; set => lastCheckPointPos = value; }

    /// <summary>
    /// Makes the GameManager able to stay the same whenever the player reloads the scene.
    /// </summary>
    private void Awake()
    {
        // Checks if there is no instance of the GameManager in the scene
        if (instance == null)
        {
            // Makes the instance 
            instance = this;
            // Makes it so this object does not get destroyed on load
            DontDestroyOnLoad(instance);
        }
        else
        {
            // Makes it so there aren't multiple instances of the Game Manager
            Destroy(gameObject);
        }
    }

    public void NewLevel()
    {
        LastCheckPointPos = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Checks if the player is back in the menu so it can reset the players position.
    /// </summary>
    public void Update()
    {
        // Makes a variable have the data of the active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Makes a string have the text of the current scenes name
        string sceneName = currentScene.name;
        // Checks if the scene has the name "Menu"
        if (sceneName == "Menu")
        {
            // Sets the player back to the position they start in in each level.
            LastCheckPointPos = new Vector3(0, 0, 0);
        }
    }
}