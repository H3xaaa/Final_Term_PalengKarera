using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import SceneManagement namespace

public class GameManager : MonoBehaviour
{
    // Function to quit the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Function to load a scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}   