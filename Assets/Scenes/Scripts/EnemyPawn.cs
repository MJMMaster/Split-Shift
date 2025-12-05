using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SaveableObject))]
public class EnemyPawn : MonoBehaviour, ISaveable
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 5f;

    [Header("Flip Settings")]
    public float flipCooldown = 0.2f;

    private Vector3 startPos;
    private int direction = 1;
    private Rigidbody rb;
    private bool canFlip = true;

    private SaveableObject saveable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        saveable = GetComponent<SaveableObject>();
        startPos = transform.position;

        // Remove negative scale
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
    }

    private void FixedUpdate() => Patrol();

    private void Patrol()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = moveSpeed * direction;
        rb.linearVelocity = vel;

        if (canFlip && Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
            FlipDirection();
    }

    private void FlipDirection()
    {
        direction *= -1;
        FlipByRotation();

        canFlip = false;
        Invoke(nameof(ResetFlip), flipCooldown);
    }

    private void ResetFlip() => canFlip = true;

    private void FlipByRotation() =>
        transform.rotation = Quaternion.AngleAxis(direction == 1 ? 0f : 180f, transform.up);

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPos : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center + Vector3.right * patrolDistance,
                        center + Vector3.left * patrolDistance);

        Gizmos.DrawWireSphere(center + Vector3.right * patrolDistance, 0.15f);
        Gizmos.DrawWireSphere(center + Vector3.left * patrolDistance, 0.15f);
    }

    // =========================
    // ISaveable Implementation
    // =========================
    [Serializable]
    public struct EnemyPawnSaveData
    {
        public Vector3 position;
        public int direction;
    }

    public object CaptureState()
    {
        return new EnemyPawnSaveData
        {
            position = transform.position,
            direction = direction
        };
    }

    public void RestoreState(object state)
    {
        if (state is EnemyPawnSaveData data)
        {
            transform.position = data.position;
            direction = data.direction;
            FlipByRotation();
        }
    }

    public string GetUniqueID() => saveable.UniqueID;
}