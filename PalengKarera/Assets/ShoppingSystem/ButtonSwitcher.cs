using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitcher : MonoBehaviour
{
    [Header("UI Buttons")]
    public GameObject pushButton; // Push Button
    public GameObject buyButton;  // Buy Button

    [Header("Interaction Settings")]
    public float interactionDistance = 3f; // Detection distance
    public LayerMask interactableLayer; // Layer for interactable items
    public Transform playerCamera; // Camera or crosshair

    [Header("Panels")]
    public GameObject mainPanel; // Main UI Panel
    public GameObject itemDescriptionPanel; // Item Description Panel

    private ItemObject currentItemObject; // Cached item being looked at

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
            if (hit.collider.CompareTag("Item"))
            {
                currentItemObject = hit.collider.GetComponent<ItemObject>();
                ShowBuyButton();

                if (Input.GetKeyDown(KeyCode.E)) // Optional key press
                {
                    BuyItem();
                }
            }
            else
            {
                currentItemObject = null;
                ShowPushButton();
            }
        }
        else
        {
            currentItemObject = null;
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

    // Called when Buy Button is clicked
    public void BuyItem()
    {
        if (currentItemObject != null)
        {
            Debug.Log("Buy button clicked on: " + currentItemObject.itemData.itemName);

            // Send data to ShoppingUIManager to update UI panel
            ShoppingUIManager.Instance.ShowItemDetails(currentItemObject.itemData);

            // Show item description panel
            ShowItemDescription();
        }
    }

    void ShowItemDescription()
    {
        mainPanel.SetActive(false);
        itemDescriptionPanel.SetActive(true);
    }

    public void CloseDescription()
    {
        itemDescriptionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
