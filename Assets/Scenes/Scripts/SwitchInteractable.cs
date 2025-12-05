using UnityEngine;
using UnityEngine.Events;

public class SwitchInteractable : InteractableBase, IInteractable
{
    [Header("Switch Settings")]
    public bool isOn = false;
    public bool oneTimeUse = false;

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
        if (oneTimeUse && hasBeenUsed) return;

        isOn = !isOn;
        hasBeenUsed = true;

        Debug.Log($"{name} switched {(isOn ? "ON" : "OFF")}");
        MessageDisplay.Instance?.ShowMessage($"{name} switched {(isOn ? "ON" : "OFF")}");
        UpdateSwitchColor();

        // Call linked door if assigned
        if (linkedDoor != null)
        {
            if (isOn) linkedDoor.OpenDoor();
            else linkedDoor.CloseDoor();
        }

        // Trigger UnityEvents
        if (isOn) OnSwitchOn?.Invoke();
        else OnSwitchOff?.Invoke();
    }

    private void UpdateSwitchColor()
    {
        if (switchRenderer != null)
            switchRenderer.material.color = isOn ? Color.green : Color.red;
    }
}