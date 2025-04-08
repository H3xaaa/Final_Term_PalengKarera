using TMPro;
using UnityEngine;

public class ShoppingUIManager : MonoBehaviour
{
    public static ShoppingUIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI itemNameText, avgPriceText, priceText, stockText, qtyText, totalText, warningText;
    public GameObject itemDescPanel;

    private ItemData currentItem;
    private int currentQty = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowItemDetails(ItemData item)
    {
        currentItem = item;
        currentQty = 1;

        itemNameText.text = item.itemName;
        avgPriceText.text = $"₱{item.averagePrice}";
        priceText.text = $"₱{item.price}";
        stockText.text = $"Stock Left: {item.stock}";
        qtyText.text = currentQty.ToString();
        totalText.text = $"₱{item.price * currentQty}";
        warningText.text = "";

        itemDescPanel.SetActive(true);
    }

    public void IncreaseQty()
    {
        if (currentItem == null) return;

        if (currentQty < currentItem.stock)
        {
            currentQty++;
            warningText.text = "";
            UpdateQtyDisplay();
        }
        else
        {
            warningText.text = "You exceeded the stock!";
        }
    }

    public void DecreaseQty()
    {
        if (currentItem == null) return;

        if (currentQty > 1)
        {
            currentQty--;
            warningText.text = "";
            UpdateQtyDisplay();
        }
    }

    public void BuyItem()
    {
        if (currentItem == null) return;

        Debug.Log($"Trying to purchase {currentQty}x {currentItem.itemName}");

        if (currentQty <= currentItem.stock)
        {
            currentItem.stock -= currentQty;

            ShoppingListManager.Instance.RegisterPurchase(currentItem, currentQty);

            ShowItemDetails(currentItem); // Refresh UI after buying
        }
    }

    private void UpdateQtyDisplay()
    {
        qtyText.text = currentQty.ToString();
        totalText.text = $"₱{currentItem.price * currentQty}";
        stockText.text = $"Stock Left: {currentItem.stock}";
    }
}
