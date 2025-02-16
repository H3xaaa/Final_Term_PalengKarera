using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Assign the panel here

    public void LoadSceneWithFakeLoading(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        loadingScreen.SetActive(true); // Show the loading panel
        yield return new WaitForSeconds(3f); // Fake 3-second loading time
        SceneManager.LoadScene(sceneName); // Load the next scene
    }
}
