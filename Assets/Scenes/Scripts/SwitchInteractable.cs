using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SwitchInteractable : InteractableBase, IInteractable
{
    [Header("Switch Settings")]
    public bool isOn = false;
    public bool oneTimeUse = false;

    [Header("Audio")]
    public AudioClip failClip;
    public AudioClip passClip;

    [Header("Item Requirement (Optional)")]
    public string requiredItemID;
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
        Debug.Log("SwitchInteractable Interact() called!");
        Debug.Log("Interact() called on switch");

        // One-time use protection
        if (oneTimeUse && hasBeenUsed)
            return;

        // Item check
        if (!string.IsNullOrEmpty(requiredItemID))
        {
            if (!PlayerInventory.Instance.HasItem(requiredItemID))
            {
                // Show message first
                MessageDisplay.Instance?.ShowMessage("Missing item: " + requiredItemID);

                // Play fail clip on next frame to ensure it works
                StartCoroutine(PlayClipNextFrame(failClip));
                return;
            }

            if (consumeItem)
                PlayerInventory.Instance.RemoveItem(requiredItemID);
        }

        // SUCCESS
        isOn = !isOn;
        hasBeenUsed = true;

        UpdateSwitchColor();

        // Door logic
        if (linkedDoor != null)
        {
            if (isOn) linkedDoor.OpenDoor();
            else linkedDoor.CloseDoor();
        }

        // Events
        if (isOn) OnSwitchOn?.Invoke();
        else OnSwitchOff?.Invoke();

        // Play success clip on next frame
        StartCoroutine(PlayClipNextFrame(passClip));
    }

    private void UpdateSwitchColor()
    {
        if (switchRenderer != null)
            switchRenderer.material.color = isOn ? Color.green : Color.red;
    }

    // =========================
    // Audio playback coroutine
    // =========================
    private IEnumerator PlayClipNextFrame(AudioClip clip)
    {
        yield return null; // wait 1 frame

        Debug.Log("Attempting to play clip: " + (clip ? clip.name : "NULL"));

        if (AudioManager.Instance != null)
        {
            Debug.Log("AudioManager instance found, SFX source: " + (AudioManager.Instance.sfxSource ? "OK" : "NULL"));
            AudioManager.Instance.PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager not ready, using fallback AudioSource for clip: " + (clip ? clip.name : "NULL"));
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = transform.position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.spatialBlend = 0f;
            aSource.volume = 1f;
            aSource.clip = clip;
            aSource.Play();
            Destroy(tempGO, clip.length + 0.1f);
        }
    }
}