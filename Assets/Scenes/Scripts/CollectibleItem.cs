using System;
using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class CollectibleItem : InteractableBase, IInteractable, ISaveable
{
    [Header("Item Settings")]
    public string itemID = "Keycard";

    [Header("Audio")]
    public AudioClip collectClip;

    private bool collected = false;
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        if (collected) return;

        collected = true;

        PlayerInventory.Instance.AddItem(itemID);

        Debug.Log($"Collected: {itemID}");
        MessageDisplay.Instance?.ShowMessage($"Collected: {itemID}");

        //  Play collect sound
        if (collectClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectClip);
        }

        // Hide the object AFTER audio is triggered
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