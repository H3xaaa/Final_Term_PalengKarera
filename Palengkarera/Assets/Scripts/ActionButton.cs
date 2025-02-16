using UnityEngine;
using UnityEngine.UI; // For UI elements
using TMPro; // If you're using TextMeshPro

public class ActionButton : MonoBehaviour
{
    public Text buttonText; // Assign the button's Text component in the inspector
    public string defaultText = "Push";
    public string interactText = "Buy";

    private int npcCount = 0; // Track the number of NPCs inside the range

    void Start()
    {
        if (buttonText != null)
        {
            buttonText.text = defaultText;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcCount++;
            UpdateButtonText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcCount--;
            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = npcCount > 0 ? interactText : defaultText;
        }
    }
}
