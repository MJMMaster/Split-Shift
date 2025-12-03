using UnityEngine;

public class InternPawn : Pawn
{
    [Header("Intern Settings")]
    public float interactDistance = 1.25f;
    public LayerMask interactableMask;

    private Rigidbody rb;
    private InteractableBase currentInteractable = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 2.5D movement: X only
        Vector3 velocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = velocity;
        HandleFlip(); // Flip sprite based on direction
    }
    private void HandleFlip()
    {
        if (moveDirection.x > 0)  // moving right
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveDirection.x < 0)  // moving left
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    private void Update()
    {
        DetectInteractable();

        // Interact only if we currently face one
        if (currentInteractable != null && Input.GetButtonDown("Submit"))
        {
            currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.right * Mathf.Sign(transform.localScale.x);

        if (Physics.Raycast(origin, direction, out hit, interactDistance, interactableMask))
        {
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();

            if (interactable != null)
            {
                // Handle newly detected interactable
                if (interactable != currentInteractable)
                {
                    HideAllPrompts();
                    currentInteractable = interactable;
                    currentInteractable.ShowPrompt();
                }

                return; // Don't hide prompts if valid hit
            }
        }

        // No interactable hit: clear any existing prompt
        if (currentInteractable != null)
        {
            currentInteractable.HidePrompt();
            currentInteractable = null;
        }
    }

    private void HideAllPrompts()
    {
        var interactables = FindObjectsByType<InteractableBase>(FindObjectsSortMode.None);

        foreach (var i in interactables)
        {
            i.HidePrompt();
        }
    }

    public override void Interact()
    {
        // Unused now — we rely on Update() input
        // You can leave this empty or remove it
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f,
                       transform.right * Mathf.Sign(transform.localScale.x) * interactDistance);
    }
}