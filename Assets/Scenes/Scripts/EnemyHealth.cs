using UnityEngine;
using System;

[RequireComponent(typeof(SaveableObject))]
public class EnemyHealth : Health, ISaveable
{
    [Header("Optional Health Bar")]
    public HealthBarSpawner healthBarSpawner;

    [Header("Intern Item Drop")]
    public bool dropsInternItem = false;
    public string internItemID;

    private bool stateRestored = false;
    private SaveableObject saveable;

    protected override void Awake()
    {
        base.Awake();

        saveable = GetComponent<SaveableObject>();

        if (healthBarSpawner == null)
            healthBarSpawner = GetComponent<HealthBarSpawner>();

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
        gameObject.SetActive(false);
        healthBarSpawner?.gameObject.SetActive(false);

        // Trigger Intern Item Drop
        if (dropsInternItem && !string.IsNullOrEmpty(internItemID))
        {
            GameManager.Instance.TriggerInternItemSpawn(internItemID);
            MessageDisplay.Instance?.ShowMessage("Something changed elsewhere...");
            Debug.Log($"[Enemy Drop] Intern item triggered: {internItemID}");
        }

        GameManager.Instance?.SaveSceneState();
    }

    protected override void OnDeath()
    {
        Debug.Log("Enemy died!");
        // Everything needed is handled in Die(), no need for DropItem() call.
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
            MarkStateRestored();

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