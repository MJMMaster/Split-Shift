using UnityEngine;
using System;

[RequireComponent(typeof(SaveableObject))]
public class EnemyHealth : Health, ISaveable
{
    [Header("Optional Health Bar")]
    public HealthBarSpawner healthBarSpawner;
    
    private bool stateRestored = false;
    private SaveableObject saveable;

    protected override void Awake()
    {
        base.Awake();

        saveable = GetComponent<SaveableObject>();

        if (healthBarSpawner == null)
            healthBarSpawner = GetComponent<HealthBarSpawner>();
        // Only initialize health if not restored from save
        if (!stateRestored && currentHealth <= 0f)
            currentHealth = maxHealth;
    }

    public void MarkStateRestored() => stateRestored = true;
    protected override void OnDamaged(float amount)
    {
        base.OnDamaged(amount);
        Debug.Log($"Enemy took {amount} damage! Remaining health: {currentHealth}");
        GameManager.Instance?.SaveSceneState();
    }

    protected override void Die()
    {
        base.Die();
        gameObject.SetActive(false); // Disable instead of destroy
        healthBarSpawner?.gameObject.SetActive(false);
        GameManager.Instance?.SaveSceneState();
    }

    protected override void OnDeath()
    {
        Debug.Log("Enemy died!");
    }

    // =========================
    // ISaveable Implementation
    // =========================
    public object CaptureState()
    {
        return new EnemyHealthSaveData
        {
            currentHealth = currentHealth,
            isActive = gameObject.activeSelf
        };
    }

    public void RestoreState(object state)
    {
        if (state is EnemyHealthSaveData data)
        {
            currentHealth = data.currentHealth;
            gameObject.SetActive(data.isActive);

            // Mark that state was restored to prevent Awake from overwriting
            MarkStateRestored();

            // Update health bar if any
            healthBarSpawner?.GetComponent<HealthBar>()?.Initialize(this);

            if (currentHealth <= 0)
                Die();
        }
        else
        {
            Debug.LogError($"EnemyHealth.RestoreState received invalid state: {state?.GetType()}");
        }
    }

    public string GetUniqueID() => saveable?.UniqueID;
}

[Serializable]
public struct EnemyHealthSaveData
{
    public float currentHealth;
    public bool isActive;
}