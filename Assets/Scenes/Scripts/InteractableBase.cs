using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Prompt")]
    public GameObject promptPrefab;
    protected GameObject promptInstance;

    [SerializeField] private Transform promptAnchor;

    protected virtual void Start()
    {
        if (promptAnchor == null)
        {
            promptAnchor = this.transform;
        }
    }

    public void ShowPrompt()
    {
        if (promptInstance == null && promptPrefab != null)
        {
            promptInstance = Instantiate(promptPrefab, promptAnchor.position, Quaternion.identity);
            promptInstance.transform.SetParent(promptAnchor);
        }
    }

    public void HidePrompt()
    {
        if (promptInstance != null)
        {
            Destroy(promptInstance);
        }
    }

    public virtual Transform GetPromptAnchor() => promptAnchor;

    // From IInteractable
    public abstract void Interact();

    private void OnDestroy()
    {
        HidePrompt();
    }
}