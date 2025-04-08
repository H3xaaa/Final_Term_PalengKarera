using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Shopping/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float price;
    public float averagePrice;
    public int stock;
}
