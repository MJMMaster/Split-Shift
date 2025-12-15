using System;
using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class InternWallSwitch : SwitchInteractable, ISaveable
{
    [Header("Target Wall Unique ID (Hero Scene)")]
    public string wallTargetID;

    [Header("Switch Settings")]
    public bool consumeItem = false;
    public string requiredItemID;

    private bool hasBeenUsed = false;
    private SaveableObject saveable;

    private void OnEnable()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        base.Interact();
        if (hasBeenUsed) return;

        if (!string.IsNullOrEmpty(requiredItemID) &&
            !PlayerInventory.Instance.HasItem(requiredItemID))
        {
            MessageDisplay.Instance?.ShowMessage("Need keycard.");
            return;
        }

        if (consumeItem)
            PlayerInventory.Instance.RemoveItem(requiredItemID);

        //  CORRECT CALL

        hasBeenUsed = true;
        GameManager.Instance.TriggerHeroWallRemoval(wallTargetID);
        GameManager.Instance.SaveSceneState();

        MessageDisplay.Instance?.ShowMessage("Something changed elsewhere...");
    }

    // =========================
    // ISaveable
    // =========================
    [Serializable]
    public struct SwitchSaveData
    {
        public bool hasBeenUsed;
    }

    public object CaptureState()
    {
        return new SwitchSaveData { hasBeenUsed = this.hasBeenUsed };
    }

    public void RestoreState(object state)
    {
        if (state is SwitchSaveData data)
            hasBeenUsed = data.hasBeenUsed;
    }

    public string GetUniqueID()
    {
        return saveable.UniqueID;
    }
}
