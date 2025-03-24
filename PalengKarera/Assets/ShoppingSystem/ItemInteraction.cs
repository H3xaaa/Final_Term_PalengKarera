using UnityEngine;
using UnityEngine.UI;

public class ItemInteraction : MonoBehaviour
{
    public Camera fpsCamera;
    public RawImage crosshair;
    public Button pushButton;
    public Button buyButton;
    public GameObject itemPanel;
    public Text itemNameText;
    public Text priceText;
    public Text stockText;
    public Image itemImage;
    public Button buyConfirmButton;

    private RaycastHit hit;
    private ItemData currentItem;
    private bool isHoveringItem = false;

    void Update()
    {
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, 5f))
        {
            if (hit.collider.CompareTag("Item"))
            {
                currentItem = hit.collider.GetComponent<ItemData>();
                pushButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(true);
                isHoveringItem = true;

                if (Input.GetButtonDown("Fire1")) // Left-click to open
                {
                    ShowItemPanel(currentItem);
                }
            }
            else
            {
                ResetButtons();
            }
        }
        else
        {
            ResetButtons();
        }
    }

    void ResetButtons()
    {
        pushButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(false);
        isHoveringItem = false;
    }

    void ShowItemPanel(ItemData item)
    {
        itemPanel.SetActive(true);
        itemNameText.text = item.itemName;
        priceText.text = "₱" + item.price.ToString();
        stockText.text = item.stockLeft.ToString() + " pcs";
        itemImage.sprite = item.itemSprite;

        buyConfirmButton.onClick.RemoveAllListeners();
        buyConfirmButton.onClick.AddListener(() => PurchaseItem(item));
    }

    void PurchaseItem(ItemData item)
    {
        if (item.stockLeft > 0 && GameManager.instance.balance >= item.price)
        {
            GameManager.instance.balance -= item.price;
            item.stockLeft--;
            stockText.text = item.stockLeft.ToString() + " pcs";
            GameManager.instance.UpdateBalanceUI();
            MarkItemAsBought(item);

            if (item.stockLeft == 0)
            {
                stockText.text = "OUT OF STOCK";
            }
        }
    }

    void MarkItemAsBought(ItemData item)
    {
        item.isBought = true;
        item.itemOverlay.SetActive(true); // Show red line or grayed overlay
    }
}
