using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private HashSet<string> items = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            Debug.LogWarning("Attempted to add NULL or empty itemID.");
            return;
        }

        if (items.Add(itemID))
        {
            Debug.Log($"[Inventory] Added: {itemID}");
            MessageDisplay.Instance?.ShowMessage($"[Inventory] Added: {itemID}");
        }
        else
        {
            Debug.Log($"[Inventory] Already have: {itemID}");
            MessageDisplay.Instance?.ShowMessage($"[Inventory] Already have: {itemID}");
        }
    }

    public bool RemoveItem(string itemID)
    {
        if (items.Remove(itemID))
        {
            Debug.Log($"[Inventory] Removed: {itemID}");
            MessageDisplay.Instance?.ShowMessage($"[Inventory] Removed: {itemID}");
            return true;
        }

        Debug.LogWarning($"[Inventory] Tried to remove missing item: {itemID}");
        return false;
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    public void PrintInventory()
    {
        Debug.Log($"[Inventory] Items: {string.Join(", ", items)}");
    }
}