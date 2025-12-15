using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class HeroHealth : Health, ISaveable
{
    [Header("Damage Settings")]
    public float touchDamage = 10f;
    public float touchCooldown = 0.5f;

    private float lastTouchTime = -999f;
    private HeroPawn movement;
    private SaveableObject saveable;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<HeroPawn>();
        saveable = GetComponent<SaveableObject>();
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

        transform.position = position;
        Revive(health);

        if (movement != null)
            movement.enabled = true;
    }

    // =========================
    // ISaveable IMPLEMENTATION
    // =========================

    [System.Serializable]
    public struct HeroHealthSaveData
    {
        public float currentHealth;
        public bool isDead;
    }

    public object CaptureState()
    {
        return new HeroHealthSaveData
        {
            currentHealth = currentHealth,
            isDead = IsDead
        };
    }

    public void RestoreState(object state)
    {
        if (state is HeroHealthSaveData data)
        {
            Revive(data.currentHealth);

            if (movement != null)
                movement.enabled = !data.isDead;
        }
    }

    public string GetUniqueID() => saveable.UniqueID;
}