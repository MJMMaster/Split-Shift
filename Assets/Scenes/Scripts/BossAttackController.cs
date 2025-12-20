using UnityEngine;
using System.Collections;

public class BossAttackController : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Attack Timing")]
    public float attackCooldown = 2.5f;

    [Header("Melee")]
    public float meleeRange = 2f;
    public int meleeDamage = 20;

    [Header("Ranged")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;

    [Header("Audio")]
    public AudioClip meleeClip;
    public AudioClip shootClip;

    private Transform player;
    private bool isAttacking;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("BossAttackController: Player not found!");

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (!isAttacking && PlayerInRange())
            {
                yield return StartCoroutine(PerformAttack());
            }

            yield return null;
        }
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        float distance = Vector3.Distance(transform.position, player.position);

        // Semi-random logic
        if (distance <= meleeRange && Random.value > 0.4f)
        {
            MeleeAttack();
        }
        else
        {
            RangedAttack();
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void MeleeAttack()
    {
        Debug.Log("Boss performs MELEE attack");

        AudioManager.Instance?.PlaySFX(meleeClip);

        if (Vector3.Distance(transform.position, player.position) <= meleeRange)
        {
            player.GetComponent<Health>()?.TakeDamage(meleeDamage);
        }
    }

    private void RangedAttack()
    {
        Debug.Log("Boss performs RANGED attack");

        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;

        AudioManager.Instance?.PlaySFX(shootClip);

        GameObject proj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
            rb.linearVelocity = direction * projectileSpeed;
        }
    }

    // =========================
    // GIZMOS (EDITOR VISUALS)
    // =========================
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Ranged direction
        if (projectileSpawnPoint != null && player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                projectileSpawnPoint.position,
                player.position
            );
        }
    }
}