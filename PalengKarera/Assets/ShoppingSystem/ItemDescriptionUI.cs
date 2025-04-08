using UnityEngine;
using TMPro;

public class ItemDescriptionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text balanceText;

    private float balance = 1000f; // Starting balance

    private ItemData currentItem;
    private int currentQuantity = 1;

    public void UpdateItemUI(ItemData itemData)
    {
        currentItem = itemData;
        itemNameText.text = itemData.itemName;
        priceText.text = "Price: ₱" + itemData.price.ToString("F2");
        quantityText.text = "Stock Left: " + itemData.quantity;
    }

    public void AddQuantity()
    {
        if (currentItem != null && currentQuantity < currentItem.quantity)
        {
            currentQuantity++;
            UpdateTotalPrice();
        }
    }

    public void MinusQuantity()
    {
        if (currentQuantity > 1)
        {
            currentQuantity--;
            UpdateTotalPrice();
        }
    }

    private void UpdateTotalPrice()
    {
        priceText.text = "Total: ₱" + (currentItem.price * currentQuantity).ToString("F2");
    }

    public void PurchaseItem()
    {
        if (currentItem != null && currentItem.quantity >= currentQuantity)
        {
            float totalCost = currentItem.price * currentQuantity;
            if (balance >= totalCost)
            {
                balance -= totalCost;
                currentItem.quantity -= currentQuantity;
                balanceText.text = "Balance: ₱" + balance.ToString("F2");
                UpdateItemUI(currentItem);
                Debug.Log("Purchased: " + currentItem.itemName);
            }
            else
            {
                Debug.Log("Not enough balance!");
            }
        }
    }
}
