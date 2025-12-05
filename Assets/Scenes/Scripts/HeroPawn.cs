using UnityEngine;

public class HeroPawn : Pawn
{
    [Header("Hero Settings")]
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Dash Settings")]
    public float dashDistance = 5f;          // Maximum dash distance
    public float dashCooldown = 1f;          // Time between dashes
    public float dashDuration = 0.1f;        // How long the dash lasts
    public LayerMask dashObstacleMask;       // Obstacles that block dash
    public float dashRaycastOffset = 0.6f;   // Offset so raycast doesn't hit player

    private Rigidbody rb;
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;
    private Vector3 dashVelocity;
    private Vector3 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Apply horizontal movement
        if (isDashing)
        {
            // During dash, maintain dash velocity
            velocity.x = dashVelocity.x;
        }
        else
        {
            velocity.x = moveDirection.x * moveSpeed;
        }

        rb.linearVelocity = velocity;

        HandleFlip();

        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void HandleFlip()
    {
        if (moveDirection.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveDirection.x < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public HeroCombat heroCombat; // assign in inspector
    public void PrimaryAction()
    {
        Jump();
        Debug.Log("Primary");
    }

    public void SecondaryAction()
    {
        heroCombat?.Attack(); // trigger attack on secondary input
        Debug.Log("Secondary");
    }
    public void ThirdAction()
    {
        Dash();
        Debug.Log("Third");
    }

    private void Jump()
    {
        if (!isGrounded) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Dash()
    {
        if (!canDash || moveDirection.x == 0) return;

        dashDirection = new Vector3(Mathf.Sign(moveDirection.x), 0, 0);

        // Raycast to detect obstacles
        Vector3 rayOrigin = transform.position + dashDirection * dashRaycastOffset;
        float maxDistance = dashDistance;

        if (Physics.Raycast(rayOrigin, dashDirection, out RaycastHit hit, dashDistance, dashObstacleMask))
        {
            maxDistance = hit.distance - 0.1f;
            if (maxDistance < 0f) maxDistance = 0f;
        }

        // Calculate dash velocity
        dashVelocity = dashDirection * (maxDistance / dashDuration);
        isDashing = true;
        canDash = false;

        // End dash after dashDuration
        Invoke(nameof(EndDash), dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        dashVelocity = Vector3.zero;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Ground check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        // Dash raycast
        if (moveDirection != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Vector3 dashRayOrigin = transform.position + new Vector3(Mathf.Sign(moveDirection.x) * dashRaycastOffset, 0, 0);
            Gizmos.DrawLine(dashRayOrigin, dashRayOrigin + dashDirection * dashDistance);
        }
    }
}