using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShoppingListManager : MonoBehaviour
{
    public static ShoppingListManager Instance;

    [System.Serializable]
    public class ShoppingRequirement
    {
        public ItemData itemData;
        public int requiredQty;
        public int boughtQty;
        public TextMeshProUGUI displayText;
    }

    public List<ShoppingRequirement> shoppingRequirements = new List<ShoppingRequirement>();
    public GameObject listItemPrefab;
    public Transform listPanel;
    public TMP_Text dishNameText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DisplayShoppingList();
    }

    void DisplayShoppingList()
    {

        // Show dish name from the GameObject name
        if (dishNameText != null)
        {
            dishNameText.text = gameObject.name;
        }

        foreach (Transform child in listPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in shoppingRequirements)
        {
            var listObj = Instantiate(listItemPrefab, listPanel);
            var tmp = listObj.GetComponent<TextMeshProUGUI>();

            tmp.text = $"{entry.itemData.itemName} x{entry.requiredQty}";
            entry.displayText = tmp;
        }
    }

    public void RegisterPurchase(ItemData item, int qty)
    {
        Debug.Log($"Trying to purchase {qty}x {item.itemName}");

        foreach (var entry in shoppingRequirements)
        {
            Debug.Log($"Checking list entry: {entry.itemData.itemName} | Required: {entry.requiredQty} | Bought: {entry.boughtQty}");

            if (entry.itemData == item)
            {
                entry.boughtQty += qty;
                int left = Mathf.Max(entry.requiredQty - entry.boughtQty, 0);

                Debug.Log($"Matched item: {item.itemName}, New Bought Qty: {entry.boughtQty}, Left: {left}");

                if (entry.boughtQty >= entry.requiredQty)
                {
                    entry.displayText.text = $"<s>{item.itemName} x{entry.requiredQty}</s>";
                    entry.displayText.color = Color.gray;
                }
                else
                {
                    entry.displayText.text = $"{item.itemName} x{left}";
                    entry.displayText.color = Color.white;
                }

                break;
            }
        }
    }
}
