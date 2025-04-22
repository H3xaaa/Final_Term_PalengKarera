using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Introscript : MonoBehaviour
{
    [Header("Camera & Animation")]
    public Camera introCamera;
    public Animator introAnimator;
    public string animationTrigger = "PlayIntro";

    [Header("Sound")]
    public AudioClip introSound;          // Drag your MP3 AudioClip here
    public AudioSource audioSource;       // Drag your AudioSource component here

    [Header("UI Fade")]
    public Image blackBackground;
    public float timeBeforeFadeOut = 3f;
    public float fadeDuration = 1f;

    [Header("Post-Intro Actions")]
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    private Camera previousMainCamera;

    private void Start()
    {
        StartCoroutine(DelayedIntroStart());
    }

    private IEnumerator DelayedIntroStart()
    {
        yield return null; // Allow scene to initialize

        // Disable previous camera if different
        if (Camera.main != null && Camera.main != introCamera)
        {
            previousMainCamera = Camera.main;
            previousMainCamera.enabled = false;
        }

        // Set intro camera as main
        if (introCamera != null)
        {
            introCamera.enabled = true;
            introCamera.tag = "MainCamera";
        }

        // Trigger animation
        if (introAnimator != null)
        {
            introAnimator.ResetTrigger(animationTrigger);
            introAnimator.SetTrigger(animationTrigger);
        }

        // Play intro MP3 sound
        if (introSound != null && audioSource != null)
        {
            audioSource.clip = introSound;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.Play();
        }

        // Start fade sequence
        if (blackBackground != null)
        {
            blackBackground.gameObject.SetActive(true);
            StartCoroutine(PlayIntro());
        }
    }

    private IEnumerator PlayIntro()
    {
        yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));
        yield return new WaitForSeconds(timeBeforeFadeOut);
        yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));

        foreach (GameObject go in objectsToEnable)
            if (go != null) go.SetActive(true);

        foreach (GameObject go in objectsToDisable)
            if (go != null) go.SetActive(false);

        if (previousMainCamera != null)
        {
            previousMainCamera.enabled = true;
            previousMainCamera.tag = "MainCamera";
        }

        if (introCamera != null)
        {
            introCamera.enabled = false;
            introCamera.tag = "Untagged";
        }

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