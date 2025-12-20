using UnityEngine;
using System;

public abstract class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public AudioClip Ouch;
    public event Action OnDeathEvent;
    public bool IsDead { get; protected set; } = false; // protected setter

    protected virtual void Awake()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth; // only set if not restored
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        AudioManager.Instance?.PlaySFX(Ouch);
        OnDamaged(amount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    protected virtual void OnDamaged(float amount) { }

    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeathEvent?.Invoke();
        OnDeath();
    }

    protected abstract void OnDeath();

    /// <summary>
    /// Revives this entity — resets death flag and optionally health.
    /// Subclasses can call this when implementing respawn mechanics.
    /// </summary>
    public virtual void Revive(float health = -1f)
    {
        IsDead = false;
        if (health > 0f) currentHealth = Mathf.Clamp(health, 0f, maxHealth);
    }
}