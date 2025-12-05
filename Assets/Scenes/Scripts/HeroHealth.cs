using UnityEngine;

[RequireComponent(typeof(SaveableObject))] // optional if you use saving
public class HeroHealth : Health
{
    [Header("Damage Settings")]
    public float touchDamage = 10f;
    public float touchCooldown = 0.5f;

    private float lastTouchTime = -999f;
    private HeroPawn movement;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<HeroPawn>();
    }

    protected override void OnDamaged(float amount)
    {
        Debug.Log($"Hero took {amount} damage! Remaining: {currentHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log("Hero has died!");

        if (movement != null)
            movement.enabled = false;

        GameOverManager.Instance?.TriggerGameOver();
    }

    public void TakeContactDamage(float amount) => TakeDamage(amount);

    public void ApplyTouchDamage()
    {
        if (Time.time - lastTouchTime < touchCooldown) return;

        lastTouchTime = Time.time;
        TakeDamage(touchDamage);
    }

    /// <summary>
    /// Clean respawn function used after reloading a scene.
    /// Fully resets death flag, restores health, reenables movement,
    /// and updates health bar because Revive() triggers internal state resets.
    /// </summary>
    public void Respawn(Vector3 position, float health)
    {
        Debug.Log($"Respawning hero at {position} with HP={health}");

        // Move player FIRST
        transform.position = position;

        // Reset death state & restore health
        Revive(health);

        // Ensure movement is back on
        if (movement != null)
            movement.enabled = true;
    }
}