using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public TMP_InputField inputField;  // Reference to the TMP InputField
    public Button myButton;            // Reference to the Button

    void Start()
    {
        // Disable the button initially
        myButton.interactable = false;

        // Add listener to check for input changes
        inputField.onValueChanged.AddListener(delegate { CheckInput(); });
    }

    // Check if input is empty or not
    void CheckInput()
    {
        // Enable button if input has value, disable if empty
        myButton.interactable = !string.IsNullOrEmpty(inputField.text.Trim());
    }
}
