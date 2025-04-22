using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Assign the loading screen panel in the Inspector

    public void LoadSceneWithFakeLoading(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        loadingScreen.SetActive(true); // Show the loading panel
        yield return new WaitForSeconds(3f); // Fake 3-second loading time

        // Clean up any existing Main Camera (useful if Intro or Lobby left one behind)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log("SceneLoader: Destroying leftover Main Camera: " + mainCam.name);
            Destroy(mainCam.gameObject);
        }

        SceneManager.LoadScene(sceneName); // Load the next scene
    }
}
