using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SaveableObject))]
public class CreditsSwitch : SwitchInteractable, ISaveable
{
    [Header("Credits Scene")]
    public string creditsSceneName = "Credits";

    private bool hasBeenUsed = false;
    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    public override void Interact()
    {
        if (hasBeenUsed) return;

        hasBeenUsed = true;

        Debug.Log("[CreditsSwitch] Interact called. Loading credits scene...");

        // Optional: play feedback clip
        if (passClip != null)
            StartCoroutine(PlayClipNextFrame(passClip));

        // Optional: change switch color to indicate it was used
        if (switchRenderer != null)
            switchRenderer.material.color = Color.green;

        // Load the credits scene
        SceneManager.LoadScene(creditsSceneName);
    }

    private IEnumerator PlayClipNextFrame(AudioClip clip)
    {
        yield return null;
        if (clip != null)
            AudioManager.Instance?.PlaySFX(clip);
    }

    // =========================
    // ISaveable
    // =========================
    [System.Serializable]
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
        {
            hasBeenUsed = data.hasBeenUsed;
            if (hasBeenUsed && switchRenderer != null)
                switchRenderer.material.color = Color.green;

            Debug.Log($"[CreditsSwitch] Restored state. HasBeenUsed: {hasBeenUsed}");
        }
    }

    public string GetUniqueID() => saveable.UniqueID;
}