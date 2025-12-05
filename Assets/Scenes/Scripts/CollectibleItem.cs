using System;
using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class CollectibleItem : InteractableBase, IInteractable, ISaveable
{
    [Header("Item Settings")]
    public string itemID = "Keycard";

    private bool collected = false;
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        if (collected) return;

        PlayerInventory.Instance.AddItem(itemID);
        collected = true;

        Debug.Log($"Collected: {itemID}");
        MessageDisplay.Instance?.ShowMessage($"Collected: {itemID}");


        // Hide the object
        gameObject.SetActive(false);
    }

    // ===================
    // ISaveable interface
    // ===================
    public object CaptureState()
    {
        return new CollectibleSaveData
        {
            collected = this.collected,
            isActive = gameObject.activeSelf
        };
    }

    public void RestoreState(object state)
    {
        var data = (CollectibleSaveData)state;
        collected = data.collected;

        // Restore active state correctly
        gameObject.SetActive(data.isActive);
    }

    public string GetUniqueID()
    {
        return saveable.UniqueID;
    }
}

[Serializable]
public struct CollectibleSaveData
{
    public bool collected;
    public bool isActive;
}