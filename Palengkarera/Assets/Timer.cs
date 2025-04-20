using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public GameObject targetObject;        // The object to watch
    public float countdownDuration = 10f;  // Duration in seconds

    [Header("UI Display (Optional)")]
    public Text legacyText;               // Optional: Unity UI Text (legacy)
    public TextMeshProUGUI tmpText;       // Optional: TextMeshPro component

    private float currentTime;
    private bool timerStarted = false;

    void Update()
    {
        // Start the timer when the target object is active
        if (targetObject != null && targetObject.activeInHierarchy && !timerStarted)
        {
            StartTimer();
        }

        // Run the countdown
        if (timerStarted)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0); // Clamp to 0

            int minutes = Mathf.FloorToInt(currentTime / 60);  // Get minutes
            int seconds = Mathf.FloorToInt(currentTime % 60);  // Get seconds

            string timeText = string.Format("{0:D2}:{1:D2}", minutes, seconds); // Format as MM:SS

            // Update whichever text UI is set
            if (legacyText != null)
                legacyText.text = timeText;

            if (tmpText != null)
                tmpText.text = timeText;

            if (currentTime <= 0)
            {
                timerStarted = false;
                Debug.Log("Countdown finished!");
                // Add logic here when countdown ends
            }
        }
    }

    public void StartTimer()
    {
        currentTime = countdownDuration;
        timerStarted = true;
        Debug.Log("Countdown started!");
    }
}
