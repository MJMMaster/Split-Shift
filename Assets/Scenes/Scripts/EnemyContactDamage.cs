using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public string playerTag = "Player";
    public float attackCooldown = 1f; // seconds between damage

    private float lastAttackTime = 0f;

    private void DealDamage(Collider player)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (player.TryGetComponent(out HeroHealth hero))
        {
            hero.TakeDamage(damage); // Make sure your HeroHealth has TakeDamage method
            lastAttackTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            DealDamage(collision.collider);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            DealDamage(collision.collider);
        }
    }
}