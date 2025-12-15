using UnityEngine;
using UnityEngine.Events;

public class SwitchInteractable : InteractableBase, IInteractable
{
    [Header("Switch Settings")]
    public bool isOn = false;
    public bool oneTimeUse = false;

    [Header("Item Requirement (Optional)")]
    public string requiredItemID;   // Leave empty for normal switch
    public bool consumeItem = false;

    [Header("Visual Feedback")]
    public Renderer switchRenderer;

    [Header("Linked Door (Optional)")]
    public DoorController linkedDoor;

    [Header("Events")]
    public UnityEvent OnSwitchOn;
    public UnityEvent OnSwitchOff;

    private bool hasBeenUsed = false;

    public override void Interact()
    {
        //  One-time use protection
        if (oneTimeUse && hasBeenUsed)
            return;

        //  Item lock check
        if (!string.IsNullOrEmpty(requiredItemID))
        {
            if (!PlayerInventory.Instance.HasItem(requiredItemID))
            {
                Debug.Log("Switch is locked. Missing item: " + requiredItemID);
                MessageDisplay.Instance?.ShowMessage("Missing item: " + requiredItemID);
                return;
            }

            if (consumeItem)
                PlayerInventory.Instance.RemoveItem(requiredItemID);
        }

        //  Toggle switch
        isOn = !isOn;
        hasBeenUsed = true;

        Debug.Log($"{name} switched {(isOn ? "ON" : "OFF")}");
        MessageDisplay.Instance?.ShowMessage($"{name} switched {(isOn ? "ON" : "OFF")}");

        UpdateSwitchColor();

        //  Linked door logic
        if (linkedDoor != null)
        {
            if (isOn) linkedDoor.OpenDoor();
            else linkedDoor.CloseDoor();
        }

        //  Unity Events
        if (isOn) OnSwitchOn?.Invoke();
        else OnSwitchOff?.Invoke();
    }

    private void UpdateSwitchColor()
    {
        if (switchRenderer != null)
            switchRenderer.material.color = isOn ? Color.green : Color.red;
    }
}