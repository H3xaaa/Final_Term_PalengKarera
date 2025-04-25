using TMPro;
using UnityEngine;

public class ShoppingUIManager : MonoBehaviour
{
    public static ShoppingUIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI itemNameText, avgPriceText, priceText, stockText, qtyText, totalText, warningText;
    public TextMeshProUGUI balanceText; // NEW: Reference to show current balance
    public GameObject itemDescPanel;

    [Header("Player Data")]
    public float playerBalance = 1000f; // NEW: Starting balance (editable in Inspector)

    private ItemData currentItem;
    private int currentQty = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateBalanceDisplay(); // NEW: Show balance on start
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

        float totalCost = currentItem.price * currentQty;

        if (currentQty > currentItem.stock)
        {
            warningText.text = "Not enough stock!";
            return;
        }

        if (playerBalance < totalCost)
        {
            warningText.text = "Not enough money!";
            return;
        }

        // Process purchase
        playerBalance -= totalCost; // NEW: Deduct from balance
        currentItem.stock -= currentQty;

        ShoppingListManager.Instance.RegisterPurchase(currentItem, currentQty);
        UpdateBalanceDisplay(); // NEW: Update balance UI
        UpdateQtyDisplay();
        ShowItemDetails(currentItem); // Refresh UI
    }

    private void UpdateQtyDisplay()
    {
        qtyText.text = currentQty.ToString();
        totalText.text = $"₱{currentItem.price * currentQty}";
        stockText.text = $"Stock Left: {currentItem.stock}";
    }

    private void UpdateBalanceDisplay() // NEW
    {
        balanceText.text = $"Budget: ₱{playerBalance}";
    }
}
