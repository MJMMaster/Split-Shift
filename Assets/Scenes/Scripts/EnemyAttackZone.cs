using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackCooldown = 1f;  // Time between hits
    public string playerTag = "Player";

    private float lastAttackTime = -Mathf.Infinity;

    private void OnTriggerEnter(Collider other)
    {
        TryAttack(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryAttack(other);
    }

    private void TryAttack(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Check cooldown
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Attempt to deal damage
        if (other.TryGetComponent(out HeroHealth hero))
        {
            hero.TakeDamage(attackDamage);
            lastAttackTime = Time.time;  // Reset cooldown
        }
    }
}