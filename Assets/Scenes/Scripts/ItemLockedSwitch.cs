using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(SaveableObject))]
public class ItemLockedSwitch : SwitchInteractable, ISaveable
{
    [Header("Connected Doors")]
    public DoorController[] doors;


    private bool hasBeenUsed = false;      // switch has been used at least once
    private bool doorsUnlocked = false;    // doors are unlocked
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        Debug.Log("ItemLockedSwitch Interact() called!");

        // If doors are not unlocked yet, try unlocking first
        if (!doorsUnlocked)
        {
            TryUnlockDoors();
        }
        else
        {
            // Doors are unlocked: toggle all doors
            ToggleDoors();
        }

        // Mark switch as used for visuals
        hasBeenUsed = true;
        UpdateSwitchVisuals();
    }

    private void TryUnlockDoors()
    {
        // Only check for key if required
        if (!string.IsNullOrEmpty(requiredItemID) && !PlayerInventory.Instance.HasItem(requiredItemID))
        {
            Debug.Log("Switch is locked. Missing item: " + requiredItemID);
            MessageDisplay.Instance?.ShowMessage("Switch is locked. Missing item: " + requiredItemID);

            // Play fail audio on next frame
            StartCoroutine(PlayClipNextFrame(failClip));
            return;
        }

        // Optionally consume key
        if (consumeItem)
            PlayerInventory.Instance.RemoveItem(requiredItemID);

        // Unlock all doors
        if (doors != null)
        {
            foreach (var d in doors)
                d.UnlockDoor();
        }

        doorsUnlocked = true;
        Debug.Log("Doors unlocked. You can now toggle them freely.");
        MessageDisplay.Instance?.ShowMessage("Doors unlocked. You can now toggle them freely.");

        // Play pass audio on next frame
        StartCoroutine(PlayClipNextFrame(passClip));
    }

    private void ToggleDoors()
    {
        if (doors == null) return;

        foreach (var d in doors)
        {
            if (d != null)
            {
                if (d.isOpen) d.CloseDoor();
                else d.OpenDoor();
            }
        }

        Debug.Log("Doors toggled!");

        // Optional: play a pass clip when doors are toggled
        StartCoroutine(PlayClipNextFrame(passClip));
    }

 

    // =========================
    // Audio playback helper
    // =========================
    private IEnumerator PlayClipNextFrame(AudioClip clip)
    {
        yield return null; // wait 1 frame

        if (clip == null)
        {
            Debug.LogWarning("Audio clip is null!");
            yield break;
        }

        if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
        else
        {
            // fallback temporary AudioSource
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = transform.position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.spatialBlend = 0f; // 2D sound
            aSource.volume = 1f;
            aSource.clip = clip;
            aSource.Play();
            Destroy(tempGO, clip.length + 0.1f);
        }
    }

    // =========================
    // ISaveable
    // =========================
    public object CaptureState()
    {
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
            doorsUnlocked = this.doorsUnlocked,
            doorsData = doorsData
        };
    }

    public void RestoreState(object state)
    {
        var data = (SwitchSaveData)state;
        hasBeenUsed = data.hasBeenUsed;
        doorsUnlocked = data.doorsUnlocked;

        if (doors != null && data.doorsData != null)
        {
            for (int i = 0; i < doors.Length && i < data.doorsData.Length; i++)
            {
                if (doors[i] != null)
                    doors[i].RestoreState(data.doorsData[i]);
            }
        }

        UpdateSwitchVisuals();
    }

    private void UpdateSwitchVisuals()
    {
        // Example: change color if switch has been used
        if (hasBeenUsed && switchRenderer != null)
        {
            switchRenderer.material.color = doorsUnlocked ? Color.green : Color.red;
        }
    }

    public string GetUniqueID() => saveable.UniqueID;
}

[Serializable]
public struct SwitchSaveData
{
    public bool hasBeenUsed;
    public bool doorsUnlocked;
    public DoorSaveData[] doorsData;
}