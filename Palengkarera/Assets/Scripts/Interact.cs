using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    public Button interactButton; // Drag the UI button here in the Inspector
    public Text interactButtonText; // Drag the UI button's Text component here
    public GameObject detectionZone; // Drag the child object with the BoxCollider here

    private bool isNearShop = false;

    void Start()
    {
        if (interactButtonText != null)
        {
            interactButtonText.text = "Push"; // Default text
        }
    }

    void Update()
    {
        if (isNearShop)
        {
            interactButtonText.text = "Buy";
        }
        else
        {
            interactButtonText.text = "Push";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearShop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearShop = false;
        }
    }
}