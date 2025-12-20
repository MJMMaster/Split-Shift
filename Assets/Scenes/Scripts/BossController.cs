using UnityEngine;

public class BossController : MonoBehaviour
{
    public int MaxHealth = 300;
    public int CurrentHealth;
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("Unique ID for cross-scene saving")]
    public string bossID; // assign in inspector

    private void Start()
    {
        CurrentHealth = MaxHealth;
        Debug.Log($"[BossController] {gameObject.name} started with {CurrentHealth} HP. BossID: {bossID}");
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"[BossController] {gameObject.name} took {amount} damage. CurrentHealth: {CurrentHealth}");

        AudioManager.Instance?.PlaySFX(hurtClip);

        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"[BossController] {gameObject.name} died!");

        AudioManager.Instance?.PlaySFX(deathClip);

        // Mark boss as defeated in GameManager
        if (!string.IsNullOrEmpty(bossID) && GameManager.Instance != null)
        {
            Debug.Log($"[BossController] Marking boss '{bossID}' as defeated in GameManager.");
            GameManager.Instance.MarkBossDefeated(bossID);
        }
        else
        {
            Debug.LogWarning("[BossController] Cannot mark boss as defeated: bossID or GameManager missing!");
        }

        Destroy(gameObject, 1.5f);
    }
}