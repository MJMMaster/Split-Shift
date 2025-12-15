using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class InternItemSpawner : MonoBehaviour, ISaveable
{
    [Header("Item prefab to spawn")]
    public GameObject itemPrefab;

    private SaveableObject saveable;
    private bool hasSpawned = false;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;

        string id = saveable.UniqueID;
        string internSceneName = "Intern"; // must match exactly

        if (GameManager.Instance.IsObjectComponentSaved(id, nameof(InternItemSpawner), internSceneName))
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        if (hasSpawned) return;

        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, transform.rotation);
            hasSpawned = true;

            Debug.Log($"[Intern Item] '{saveable.UniqueID}' spawned.");
            MessageDisplay.Instance?.ShowMessage("An item appeared in the Intern scene!");
        }
        else
        {
            Debug.LogWarning($"[Intern Item] No prefab assigned for '{saveable.UniqueID}'!");
        }
    }

    public object CaptureState()
    {
        return hasSpawned;
    }

    public void RestoreState(object state)
    {
        if (state is bool val)
            hasSpawned = val;
    }

    public string GetUniqueID() => saveable.UniqueID;
}