using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public int price;
    public int stockLeft;
    public Sprite itemSprite;
    public bool isBought = false;
    public GameObject itemOverlay; // Red line or grayed-out image

    void Start()
    {
        itemOverlay.SetActive(false); // Hide overlay by default
    }
}
