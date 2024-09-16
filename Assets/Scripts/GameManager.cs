using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text healthStats; // Grab the text object

    public TakeDamage takeDamage; // Grab the script for taking damage
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
    }

    // Keep track of current player health state
    void UpdateHealth()
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
