using System;
using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class ItemLockedSwitch : SwitchInteractable, ISaveable
{
    [Header("Item Requirement")]
    public string requiredItemID;
    public bool consumeItem = false;

    [Header("Connected Doors")]
    public DoorController[] doors;  // assign multiple doors in Inspector

    private bool hasBeenUsed = false;
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        // Check for item
        if (!string.IsNullOrEmpty(requiredItemID))
        {
            if (!PlayerInventory.Instance.HasItem(requiredItemID))
            {
                Debug.Log("Switch is locked. Missing item: " + requiredItemID);
                return;
            }

            // Optional: consume the key
            if (consumeItem)
            {
                PlayerInventory.Instance.RemoveItem(requiredItemID);
            }
        }

        // Unlock all connected doors
        if (doors != null)
        {
            foreach (var d in doors)
            {
                if (d != null)
                    d.UnlockDoor();
            }
        }

        // Mark as used
        hasBeenUsed = true;

        // Trigger switch behavior (color change, animation, Unity events, etc.)
        base.Interact();
    }

    // ===================
    // ISaveable interface
    // ===================
    public object CaptureState()
    {
        // Save whether the switch has been used and current state of doors
        DoorSaveData[] doorsData = null;
        if (doors != null && doors.Length > 0)
        {
            doorsData = new DoorSaveData[doors.Length];
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] != null)
                    doorsData[i] = new DoorSaveData
                    {
                        isOpen = doors[i].isOpen,
                        isLocked = doors[i].isLocked,
                        position = doors[i].transform.position
                    };
            }
        }

        return new SwitchSaveData
        {
            hasBeenUsed = this.hasBeenUsed,
            doorsData = doorsData
        };
    }

    public void RestoreState(object state)
    {
        var data = (SwitchSaveData)state;

        hasBeenUsed = data.hasBeenUsed;

        if (doors != null && data.doorsData != null)
        {
            for (int i = 0; i < doors.Length && i < data.doorsData.Length; i++)
            {
                if (doors[i] != null)
                    doors[i].RestoreState(data.doorsData[i]);
            }
        }

        // Update visuals based on current state
        if (hasBeenUsed)
            base.Interact(); // triggers color change/events if needed
    }

    public string GetUniqueID()
    {
        return saveable.UniqueID;
    }
}

// ===================
// Save data struct
// ===================
[Serializable]
public struct SwitchSaveData
{
    public bool hasBeenUsed;
    public DoorSaveData[] doorsData;
}