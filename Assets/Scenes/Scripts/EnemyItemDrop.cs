using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class EnemyItemDrop : MonoBehaviour, ISaveable
{
    [Header("Intern Item ID To Spawn")]
    public string internItemID;

    private EnemyHealth enemyHealth;
    private bool triggered = false;
    private SaveableObject saveable;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        saveable = GetComponent<SaveableObject>();
    }

    private void Update()
    {
        if (triggered || enemyHealth == null) return;

        if (enemyHealth.currentHealth <= 0)
        {
            triggered = true;

            GameManager.Instance.TriggerInternItemSpawn(internItemID);
            GameManager.Instance.SaveSceneState();

            Debug.Log($"[Enemy Drop] Intern item '{internItemID}' queued.");
            MessageDisplay.Instance?.ShowMessage("Something changed elsewhere...");
        }
    }

    // =========================
    // ISaveable
    // =========================
    public object CaptureState()
    {
        return triggered;
    }

    public void RestoreState(object state)
    {
        if (state is bool val)
            triggered = val;
    }

    public string GetUniqueID() => saveable.UniqueID;
}