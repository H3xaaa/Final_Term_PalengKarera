using UnityEngine;
using TMPro;

public class MobilePerformanceManager : MonoBehaviour
{
    [Header("TextMeshPro UI for FPS")]
    public TextMeshProUGUI fpsText;

    [Header("Dynamic Resolution Settings")]
    public int targetFrameRate = 60;
    public float minScale = 0.5f;
    public float maxScale = 1.0f;

    float timer;

    void Start()
    {
        // Set target frame rate and disable VSync
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;

        // Start at lower resolution for performance
        ScalableBufferManager.ResizeBuffers(minScale, minScale);

        // Optional: Tune Unity physics for better smoothness
        Time.fixedDeltaTime = 0.05f;
        Time.maximumDeltaTime = 0.1f;
    }

    void Update()
    {
        // FPS display
        timer += Time.unscaledDeltaTime;
        if (timer >= 0.5f && fpsText != null)
        {
            float fps = 1f / Time.unscaledDeltaTime;
            fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
            timer = 0f;
        }

        // Dynamic resolution scaling
        float currentFPS = 1f / Time.unscaledDeltaTime;
        if (currentFPS < targetFrameRate - 10)
        {
            ScalableBufferManager.ResizeBuffers(minScale, minScale);
        }
        else if (currentFPS >= targetFrameRate)
        {
            ScalableBufferManager.ResizeBuffers(maxScale, maxScale);
        }
    }
}
