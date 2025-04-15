using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewShoppingList", menuName = "ShoppingSystem/ShoppingList")]
public class ShoppingListData : ScriptableObject
{
    [System.Serializable]
    public class ShoppingItemRequirement
    {
        public ItemData itemData;
        public int requiredAmount;
    }

    public List<ShoppingItemRequirement> listEntries;
}
