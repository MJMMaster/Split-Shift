using UnityEngine;

public class HealthBarSpawner : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Vector3 offset = new Vector3(0, 2f, 0);

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

        // Spawn health bar above the target
        barInstance = Instantiate(
            healthBarPrefab,
            transform.position + offset,
            Quaternion.identity
        ).transform;

        // Make bar independent (not parented)
        barInstance.SetParent(null);

        // Initialize bar with the health reference
        var bar = barInstance.GetComponent<HealthBar>();
        bar.Initialize(health);

        // When the health dies, destroy the bar
        health.OnDeathEvent += HandleDeath;
    }

    private void LateUpdate()
    {
        if (barInstance == null) return;

        // Follow target position
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
        // If this object is destroyed normally, kill the bar as well
        if (barInstance != null)
        {
            Destroy(barInstance.gameObject);
        }
    }
}