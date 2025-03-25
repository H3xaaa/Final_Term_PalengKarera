using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ShoppingSystem/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float price;
    public int quantity;
}
