using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoad : MonoBehaviour
{
    public string sceneName; // Name of the scene to load
    public Button loadButton; // Assign the button in the Inspector

    void Start()
    {
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadScene);
        }
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set!");
        }
    }
}