using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float speed = 8f;
    public float maxBounceAngle = 60f;

    [Header("Audio")]
    public AudioClip collisionClip; // assign in Inspector
    [Range(0f, 1f)] public float collisionVolume = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start the ball moving upward with slight randomness
        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f).normalized;
        rb.linearVelocity = dir * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play collision sound at the contact point
        if (collisionClip != null)
        {
            AudioSource.PlayClipAtPoint(collisionClip, collision.contacts[0].point, collisionVolume);
        }

        if (collision.gameObject.CompareTag("Paddle"))
        {
            Paddle paddle = collision.gameObject.GetComponent<Paddle>();

            // How far from the center of the paddle did we hit?
            float hitPoint = transform.position.x - collision.transform.position.x;
            float halfWidth = collision.collider.bounds.size.x / 2f;
            float normalizedHit = Mathf.Clamp(hitPoint / halfWidth, -1f, 1f);

            // Calculate bounce angle
            float angle = normalizedHit * maxBounceAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(
                Mathf.Sin(angle),
                Mathf.Cos(angle),
                0f
            );

            // Add paddle movement influence
            direction.x += paddle.moveDirection * 0.3f;

            rb.linearVelocity = direction.normalized * speed;
        }
    }
}