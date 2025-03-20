using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        // Find the PlayerController in the scene
        playerController = FindObjectOfType<PlayerController>();
    }

    public void Restart()
    {
        // Ensure time is reset (in case the game was paused)
        Time.timeScale = 1f;

        // Reset player stats before reloading the scene
        if (playerController != null)
        {
            playerController.ResetStats();
        }

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
// Update is called once per frame
void Update()
    {
        
    }
}
