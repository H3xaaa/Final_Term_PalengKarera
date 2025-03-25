using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitcher : MonoBehaviour
{
    public GameObject pushButton; // Push Button
    public GameObject buyButton;  // Buy Button
    public float interactionDistance = 3f; // Detection distance
    public LayerMask interactableLayer; // Layer for interactable items
    public Transform playerCamera; // Camera or crosshair

    // New Panels
    public GameObject mainPanel; // Main UI Panel
    public GameObject itemDescriptionPanel; // Item Description Panel

    void Update()
    {
        CheckForObject();
    }

    void CheckForObject()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Item")) // Check if the item is detected
            {
                ShowBuyButton();
                if (Input.GetKeyDown(KeyCode.E)) // Optional: Press E to buy
                {
                    BuyItem(); // Call BuyItem if E is pressed
                }
            }
            else
            {
                ShowPushButton();
            }
        }
        else
        {
            ShowPushButton();
        }
    }

    void ShowBuyButton()
    {
        pushButton.SetActive(false);
        buyButton.SetActive(true);
    }

    void ShowPushButton()
    {
        pushButton.SetActive(true);
        buyButton.SetActive(false);
    }

    // 👇 Call this when Buy Button is clicked
    public void BuyItem()
    {
        Debug.Log("Buy button work");
        ShowItemDescription();
    }

    void ShowItemDescription()
    {
        mainPanel.SetActive(false); // Disable Main Panel
        itemDescriptionPanel.SetActive(true); // Enable Item Description Panel
    }

    public void CloseDescription()
    {
        itemDescriptionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
