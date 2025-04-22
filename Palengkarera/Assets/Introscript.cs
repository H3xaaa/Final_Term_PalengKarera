using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Introscript : MonoBehaviour
{
    [Header("Camera & Animation")]
    public Camera introCamera;                       // Camera used for intro
    public Animator introAnimator;                   // Animator component with intro animation
    public string animationTrigger = "PlayIntro";    // Trigger name to play intro animation

    [Header("Sound")]
    public AudioClip introSound;                     // Optional sound to play
    public AudioSource audioSource;                  // Audio source to play the sound

    [Header("UI Fade")]
    public Image blackBackground;                    // Black screen UI image
    public float timeBeforeFadeOut = 3f;             // Time before fading back to black
    public float fadeDuration = 1f;                  // Duration of fade in/out

    [Header("Post-Intro Actions")]
    public List<GameObject> objectsToEnable;         // Objects to enable after intro
    public List<GameObject> objectsToDisable;        // Objects to disable after intro

    private Camera previousMainCamera;

    private void OnEnable()
    {
        // Destroy leftover main camera from previous scene if not the introCamera
        if (Camera.main != null && Camera.main != introCamera)
        {
            previousMainCamera = Camera.main;
            Debug.Log("Destroying leftover main camera: " + previousMainCamera.name);
            Destroy(previousMainCamera.gameObject);
        }

        // Set intro camera as main
        if (introCamera != null)
        {
            introCamera.enabled = true;
            introCamera.tag = "MainCamera";
            Debug.Log("Intro camera enabled: " + introCamera.name);
        }

        // Play intro animation using Animator
        if (introAnimator != null)
        {
            introAnimator.ResetTrigger(animationTrigger); // Ensure it restarts
            introAnimator.SetTrigger(animationTrigger);
        }
        else
        {
            Debug.LogWarning("Intro Animator not assigned.");
        }

        // Play intro sound
        if (introSound != null && audioSource != null)
        {
            audioSource.clip = introSound;
            audioSource.Play();
            Debug.Log("Playing intro sound: " + introSound.name);
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("Audio Source is not assigned.");
        }
        else if (introSound == null)
        {
            Debug.LogWarning("Intro Sound is not assigned.");
        }

        // Begin intro sequence
        if (blackBackground != null)
        {
            blackBackground.gameObject.SetActive(true);
            StartCoroutine(PlayIntro());
        }
        else
        {
            Debug.LogWarning("Black background UI Image is not assigned.");
        }
    }

    private IEnumerator PlayIntro()
    {
        // Fade in (Black to Transparent)
        yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));

        // Wait before fading out
        yield return new WaitForSeconds(timeBeforeFadeOut);

        // Fade out (Transparent to Black)
        yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));

        // Enable objects
        foreach (GameObject go in objectsToEnable)
            if (go != null) go.SetActive(true);

        // Disable objects
        foreach (GameObject go in objectsToDisable)
            if (go != null) go.SetActive(false);

        // Disable intro camera
        if (introCamera != null)
        {
            introCamera.enabled = false;
            introCamera.tag = "Untagged";
            Debug.Log("Intro camera disabled.");
        }

        // Deactivate this script's GameObject
        gameObject.SetActive(false);
    }

    private IEnumerator FadeImage(float fromAlpha, float toAlpha, float duration)
    {
        float time = 0f;
        Color originalColor = blackBackground.color;

        while (time < duration)
        {
            float t = time / duration;
            float a = Mathf.Lerp(fromAlpha, toAlpha, t);
            blackBackground.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            time += Time.deltaTime;
            yield return null;
        }

        blackBackground.color = new Color(originalColor.r, originalColor.g, originalColor.b, toAlpha);
    }
}