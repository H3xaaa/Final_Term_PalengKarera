using UnityEngine;

public class VisualUpdater : MonoBehaviour
{
    [Header("Set this to the original full stock value")]
    public int maxStock = 100;

    [Header("Visual State Objects")]
    public GameObject fullObject;
    public GameObject halfObject;
    public GameObject emptyObject;

    private ItemObject itemObject;

    void Start()
    {
        itemObject = GetComponent<ItemObject>();

        if (itemObject == null || itemObject.itemData == null)
        {
            Debug.LogWarning("ItemObject or ItemData is not assigned.");
            enabled = false; // Stop the script
            return;
        }

        // 🔁 Reset stock to maxStock at the start of play mode
        itemObject.itemData.stock = maxStock;

        UpdateVisual();
    }

    void Update()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        int currentStock = itemObject.itemData.stock;

        if (maxStock <= 0)
        {
            Debug.LogWarning("Max stock is zero or less. Cannot compute percentage.");
            return;
        }

        float stockPercent = (float)currentStock / maxStock;

        if (stockPercent <= 0f)
        {
            SetVisualState(false, false, true); // Empty only
        }
        else if (stockPercent <= 0.8f)
        {
            SetVisualState(false, true, false); // Half only
        }
        else
        {
            SetVisualState(true, false, false); // Full only
        }
    }

    void SetVisualState(bool full, bool half, bool empty)
    {
        if (fullObject && fullObject.activeSelf != full) fullObject.SetActive(full);
        if (halfObject && halfObject.activeSelf != half) halfObject.SetActive(half);
        if (emptyObject && emptyObject.activeSelf != empty) emptyObject.SetActive(empty);
    }
}
