using UnityEngine;

public class HeroPawn : Pawn
{
    [Header("Hero Settings")]
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Move horizontally
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        rb.linearVelocity = velocity;
        HandleFlip(); // Flip sprite based on direction
        // Check grounding
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void HandleFlip()
    {
        if (moveDirection.x > 0)  // moving right
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveDirection.x < 0)  // moving left
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    public void PrimaryAction()
    {
        Jump();
        Debug.Log("Primary action success");
    }

    public void SecondaryAction()
    {
        Debug.Log("secondary action success");
        // Add ability here later
    }

    private void Jump()
    {
        if (!isGrounded) return;

        // Zero out any downward momentum so jump feels responsive
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}