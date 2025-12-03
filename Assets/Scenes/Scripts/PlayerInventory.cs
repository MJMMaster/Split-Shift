using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private HashSet<string> items = new HashSet<string>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // Inventory persists between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Adds an item ID to the inventory.
    /// </summary>
    public void AddItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            Debug.LogWarning("Attempted to add NULL or empty itemID.");
            return;
        }

        if (items.Add(itemID))
            Debug.Log($"[Inventory] Added: {itemID}");
        else
            Debug.Log($"[Inventory] Already have: {itemID}");
    }

    /// <summary>
    /// Removes an item (used for consumeItem doors).
    /// </summary>
    public bool RemoveItem(string itemID)
    {
        if (items.Remove(itemID))
        {
            Debug.Log($"[Inventory] Removed: {itemID}");
            return true;
        }

        Debug.LogWarning($"[Inventory] Tried to remove missing item: {itemID}");
        return false;
    }

    /// <summary>
    /// Checks if the inventory contains a given item ID.
    /// </summary>
    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    /// <summary>
    /// Debug helper: prints all items.
    /// </summary>
    public void PrintInventory()
    {
        Debug.Log($"[Inventory] Items: {string.Join(", ", items)}");
    }
}