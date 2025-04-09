using UnityEngine;
using System.Collections.Generic;

public class ShoppingListRandomizer : MonoBehaviour
{
    [SerializeField] private Transform shoppingListParent; // Optional: parent of all your dish lists
    private ShoppingListManager activeList;

    void Start()
    {
        List<ShoppingListManager> allLists = new List<ShoppingListManager>();

        // Find all ShoppingListManagers in scene (optionally just under a parent)
        if (shoppingListParent != null)
        {
            allLists.AddRange(shoppingListParent.GetComponentsInChildren<ShoppingListManager>(true));
        }
        else
        {
            allLists.AddRange(FindObjectsOfType<ShoppingListManager>(true));
        }

        if (allLists.Count == 0)
        {
            Debug.LogWarning("No ShoppingListManager found!");
            return;
        }

        // Disable all
        foreach (var list in allLists)
        {
            list.gameObject.SetActive(false);
        }

        // Pick random one
        int index = Random.Range(0, allLists.Count);
        activeList = allLists[index];
        activeList.gameObject.SetActive(true);

        Debug.Log("Selected dish: " + activeList.gameObject.name);
    }
}
