using UnityEngine;

public class ItemClick : MonoBehaviour
{
    [SerializeField] private ItemData itemData;  // Drag the corresponding ItemData asset here
    [SerializeField] private ItemDescriptionUI descriptionUI;

    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) // Press E to interact
        {
            ShowItemDetails();
        }
    }

    private void ShowItemDetails()
    {
        if (itemData != null && descriptionUI != null)
        {
            descriptionUI.UpdateItemUI(itemData);  // ✅ Correct method
        }
        else
        {
            Debug.LogError("ItemData or DescriptionUI is missing!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))  // ✅ Check for NPC tag
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))  // ✅ Check for NPC tag
        {
            isPlayerNear = false;
        }
    }
}
