using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(SaveableObject))]
public class BossUnlockSwitch : SwitchInteractable, ISaveable
{
    [Header("Target Boss ID (cross-scene)")]
    public string bossID;

    [Header("Connected Doors")]
    public DoorController[] doors;

    private bool doorsUnlocked = false;
    private bool hasBeenUsed = false;
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckBossState();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckBossState();
    }

    private void CheckBossState()
    {
        if (doorsUnlocked) return;

        if (!string.IsNullOrEmpty(bossID) && GameManager.Instance != null)
        {
            if (GameManager.Instance.IsBossDefeated(bossID))
            {
                Debug.Log($"[BossUnlockSwitch] Boss '{bossID}' is defeated. Unlocking switch.");
                UnlockDoors();
            }
            else
            {
                Debug.Log($"[BossUnlockSwitch] Boss '{bossID}' not defeated yet.");
            }
        }
    }

    public override void Interact()
    {
        if (!doorsUnlocked)
        {
            Debug.Log("[BossUnlockSwitch] Switch is locked, cannot interact yet.");
            MessageDisplay.Instance?.ShowMessage("This switch is locked...");
            StartCoroutine(PlayClipNextFrame(failClip));
            return;
        }

        ToggleDoors();
        hasBeenUsed = true;
        UpdateSwitchVisuals();
    }

    private void UnlockDoors()
    {
        if (doorsUnlocked) return;

        doorsUnlocked = true;

        if (doors != null)
        {
            foreach (var d in doors)
            {
                if (d != null)
                {
                    d.UnlockDoor();
                    Debug.Log("[BossUnlockSwitch] Door unlocked.");
                }
            }
        }

        if (passClip != null)
            StartCoroutine(PlayClipNextFrame(passClip));

        UpdateSwitchVisuals();

        Debug.Log("[BossUnlockSwitch] Switch unlocked because boss was defeated.");
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

        if (passClip != null)
            StartCoroutine(PlayClipNextFrame(passClip));

        Debug.Log("[BossUnlockSwitch] Doors toggled!");
        if (passClip != null)
            StartCoroutine(PlayClipNextFrame(passClip));
    }

    private IEnumerator PlayClipNextFrame(AudioClip clip)
    {
        yield return null;
        AudioManager.Instance?.PlaySFX(clip);
    }

    private void UpdateSwitchVisuals()
    {
        if (switchRenderer != null)
        {
            if (!doorsUnlocked)
                switchRenderer.material.color = Color.red;
            else if (hasBeenUsed)
                switchRenderer.material.color = Color.yellow;
            else
                switchRenderer.material.color = Color.green;
        }
    }

    // =========================
    // ISaveable
    // =========================
    [Serializable]
    public struct SwitchSaveData
    {
        public bool doorsUnlocked;
        public bool hasBeenUsed;
        public DoorSaveData[] doorsData;
    }

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
            doorsUnlocked = this.doorsUnlocked,
            hasBeenUsed = this.hasBeenUsed,
            doorsData = doorsData
        };
    }

    public void RestoreState(object state)
    {
        if (state is SwitchSaveData data)
        {
            doorsUnlocked = data.doorsUnlocked;
            hasBeenUsed = data.hasBeenUsed;

            if (doors != null && data.doorsData != null)
            {
                for (int i = 0; i < doors.Length && i < data.doorsData.Length; i++)
                {
                    if (doors[i] != null)
                        doors[i].RestoreState(data.doorsData[i]);
                }
            }

            UpdateSwitchVisuals();
            Debug.Log($"[BossUnlockSwitch] Restored state. DoorsUnlocked: {doorsUnlocked}, HasBeenUsed: {hasBeenUsed}");
        }
    }

    public string GetUniqueID() => saveable.UniqueID;
}