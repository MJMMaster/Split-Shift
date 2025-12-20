using UnityEngine;

public class HealthBarSpawner : MonoBehaviour
{
    [Header("Health Bar Prefab")]
    public GameObject healthBarPrefab;
    public Vector3 offset = new Vector3(0, 2f, 0);

    [Header("Optional Boss Target")]
    public BossController bossTarget;

    private Health health;
    private Transform barInstance;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        if (healthBarPrefab == null)
        {
            Debug.LogError("Missing health bar prefab!");
            return;
        }

        // Spawn health bar
        barInstance = Instantiate(healthBarPrefab, transform.position + offset, Quaternion.identity).transform;
        barInstance.SetParent(null); // independent in world space

        // Initialize bar
        var bar = barInstance.GetComponent<HealthBar>();
        if (bossTarget != null)
        {
            bar.InitializeBoss(bossTarget);
        }
        else if (health != null)
        {
            bar.Initialize(health);
            health.OnDeathEvent += HandleDeath; // destroy bar when health dies
        }
        else
        {
            Debug.LogWarning("No Health or BossController found on target!");
        }
    }

    private void LateUpdate()
    {
        if (barInstance == null) return;

        barInstance.position = transform.position + offset;
    }

    private void HandleDeath()
    {
        if (barInstance != null)
        {
            Destroy(barInstance.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (barInstance != null)
        {
            Destroy(barInstance.gameObject);
        }
    }
}