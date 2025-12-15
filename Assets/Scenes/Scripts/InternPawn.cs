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
        HandleFlip();
    }

    private void HandleFlip()
    {
        if (moveDirection.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveDirection.x < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable != null && Input.GetButtonDown("Submit"))
        {
            currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        //  RAYCAST NOW ALWAYS FACES NORTH (FORWARD / +Z)
        Vector3 direction = Vector3.forward;

        if (Physics.Raycast(origin, direction, out hit, interactDistance, interactableMask))
        {
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();

            if (interactable != null)
            {
                if (interactable != currentInteractable)
                {
                    HideAllPrompts();
                    currentInteractable = interactable;
                    currentInteractable.ShowPrompt();
                }

                return;
            }
        }

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
        // Unused
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        //  GIZMO NOW ALSO FACES NORTH
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f,
                       Vector3.forward * interactDistance);
    }
}