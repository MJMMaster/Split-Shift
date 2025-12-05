using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeroCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackRange = 2f; // how far the cone reaches
    [Range(1f, 180f)]
    public float attackAngle = 60f; // cone angle

    public LayerMask attackLayer;   // enemy layers

    // How many lines to draw across the cone arc for visualization
    [Header("Gizmo")]
    [Range(4, 64)] public int gizmoSegments = 20;
    public Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Attack()
    {
        // Determine facing direction based on horizontal velocity, fall back to localScale.x
        Vector3 facing = GetFacingDirection();

        // Broad-phase sphere
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, attackLayer);

        int hitCount = 0;
        foreach (var hit in hits)
        {
            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

            float angle = Vector3.Angle(facing, directionToTarget);

            if (angle <= attackAngle * 0.5f)
            {
                Health hp = hit.GetComponent<Health>();
                if (hp)
                {
                    hp.TakeDamage(attackDamage);
                    hitCount++;
                }
            }
        }

        Debug.Log($"Hero performed CONE ATTACK and hit {hitCount} enemies!");
    }

    private Vector3 GetFacingDirection()
    {
        // Prefer horizontal velocity for facing when moving
        if (rb != null)
        {
            float vx = rb.linearVelocity.x;
            if (Mathf.Abs(vx) > 0.05f)
                return new Vector3(Mathf.Sign(vx), 0f, 0f);
        }

        // Fallback: use localScale.x so the cone faces visually-correct direction
        float scaleX = transform.localScale.x;
        return new Vector3(Mathf.Sign(scaleX != 0 ? scaleX : 1f), 0f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the cone in the Scene view so designers can see it clearly
        Vector3 origin = transform.position;
        Vector3 facing = Vector3.right; // default

        // try to get runtime-facing if possible (works in play or edit mode)
        if (Application.isPlaying)
            facing = GetFacingDirection();
        else
            facing = new Vector3(Mathf.Sign(transform.localScale.x != 0 ? transform.localScale.x : 1f), 0f, 0f);

        facing.Normalize();

        Gizmos.color = gizmoColor;

        // draw outer arc lines (fan)
        float halfAngle = attackAngle * 0.5f;
        int segments = Mathf.Max(4, gizmoSegments);
        Vector3 prevPoint = origin + Quaternion.Euler(0f, -halfAngle, 0f) * facing * attackRange;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * facing;
            Vector3 nextPoint = origin + dir * attackRange;

            // outline edges
            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }

        // draw range circle for reference (wire)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}