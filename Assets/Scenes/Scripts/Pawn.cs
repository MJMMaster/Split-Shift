using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    protected Vector3 moveDirection;

    // Allows child classes to override specific movement rules.
    public virtual void Move(Vector3 direction)
    {
        moveDirection = direction;
    }

    // Interaction stub
    public virtual void Interact()
    {
        // Intentionally blank — child decides behavior
    }
}