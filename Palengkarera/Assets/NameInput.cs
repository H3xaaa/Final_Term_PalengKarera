using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInput : MonoBehaviour
{
    public TMP_InputField nameInputField; // Assign the TMP Input Field
    public TMP_Text displayText; // Assign the TMP Text to display the name

    void Start()
    {
        // Update the display text whenever the player types
        nameInputField.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(string newText)
    {
        displayText.text = newText + "'s Lobby"; // Append "'s Lobby" to the input name
    }

    public void SaveName()
    {
        Debug.Log("Player Name: " + nameInputField.text);
    }
}