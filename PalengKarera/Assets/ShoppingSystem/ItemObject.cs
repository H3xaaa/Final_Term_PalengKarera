using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;

    public void OnClick()
    {
        ShoppingUIManager.Instance.ShowItemDetails(itemData);
    }
}
