using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    [Header("Boss Reference")]
    public BossController boss;

    [Header("Damage Settings")]
    public float damageMultiplier = 1.5f; // weak point bonus

    public void TakeDamage(float incomingDamage)
    {
        if (boss == null)
        {
            Debug.LogWarning("BossWeakPoint has no BossController assigned.", this);
            return;
        }

        float finalDamage = Mathf.Round(incomingDamage * damageMultiplier);

        boss.TakeDamage((int)finalDamage);

        Debug.Log($"Boss weak point hit for {finalDamage} damage!");
    }
}