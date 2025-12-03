using UnityEngine;

public interface IInteractable
{
    void Interact();
    Transform GetPromptAnchor();   // where UI should appear
}