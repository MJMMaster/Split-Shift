using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ---------- WORLD AUTOSAVE ----------
    // sceneName -> objectID -> componentName -> saved state
    private Dictionary<string, Dictionary<string, Dictionary<string, object>>> allSceneStates = new();

    // ---------- CHECKPOINT SAVE ----------
    private PlayerCheckpointData checkpoint;

    // Player references
    private Transform playerTransform;
    private Health playerHealth;

    private readonly HashSet<string> excludedComponents = new()
    {
        "PlayerHealth",
        "HeroPawn",
        "PlayerPawn",
    };

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CachePlayerReferences();
        RestoreSceneState(scene.name);
    }

    private void CachePlayerReferences()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerHealth = playerTransform?.GetComponent<Health>();
    }

    // -----------------------------------------
    //            WORLD AUTOSAVE
    // -----------------------------------------
    public void SaveSceneState()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        var sceneObjects = new Dictionary<string, Dictionary<string, object>>();

        var allMono = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        var saveables = allMono.OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            if (saveable is MonoBehaviour mb)
            {
                SaveableObject so = mb.GetComponent<SaveableObject>();
                if (so == null) continue;

                string compName = saveable.GetType().Name;

                // Skip player health + position
                if (excludedComponents.Contains(compName))
                    continue;

                if (!sceneObjects.ContainsKey(so.UniqueID))
                    sceneObjects[so.UniqueID] = new Dictionary<string, object>();

                sceneObjects[so.UniqueID][compName] = saveable.CaptureState();
            }
        }

        allSceneStates[sceneName] = sceneObjects;
        Debug.Log($"[Autosave] Scene '{sceneName}' saved ({sceneObjects.Count} objects).");
    }

    public void RestoreSceneState(string sceneName)
    {
        if (!allSceneStates.TryGetValue(sceneName, out var sceneObjects))
            return;

        var allMono = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        var saveables = allMono.OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            if (saveable is MonoBehaviour mb)
            {
                SaveableObject so = mb.GetComponent<SaveableObject>();
                if (so == null) continue;

                string compName = saveable.GetType().Name;

                // Skip restoring player health + position
                if (excludedComponents.Contains(compName))
                    continue;

                if (allSceneStates.TryGetValue(sceneName, out var objDict))
                {
                    if (objDict.TryGetValue(so.UniqueID, out var compDict))
                    {
                        if (compDict.TryGetValue(compName, out object savedData))
                        {
                            try
                            {
                                saveable.RestoreState(savedData);
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"Error restoring {compName} on {so.UniqueID}: {e}");
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"[Autosave] Scene '{sceneName}' restored ({sceneObjects.Count} objects).");
    }

    // -----------------------------------------
    //            CHECKPOINT SYSTEM
    // -----------------------------------------
    public void SaveCheckpoint()
    {
        if (playerTransform == null)
            CachePlayerReferences();

        checkpoint = new PlayerCheckpointData
        {
            sceneName = SceneManager.GetActiveScene().name,
            position = playerTransform.position,
            rotation = playerTransform.eulerAngles,
            health = playerHealth.currentHealth
        };

        Debug.Log("[Checkpoint] Saved player position + health.");
    }

    public void LoadCheckpoint()
    {
        if (checkpoint == null)
        {
            Debug.LogWarning("[Checkpoint] No checkpoint to load!");
            return;
        }

        SceneManager.LoadScene(checkpoint.sceneName);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            CachePlayerReferences();

            playerTransform.position = checkpoint.position;
            playerTransform.eulerAngles = checkpoint.rotation;
            playerHealth.currentHealth = checkpoint.health;

            Debug.Log("[Checkpoint] Player restored.");
        };
    }

    // -----------------------------------------
    // Utility
    // -----------------------------------------
    public object GetObjectComponentState(string uniqueID, string componentName, string sceneName = null)
    {
        sceneName ??= SceneManager.GetActiveScene().name;
        if (allSceneStates.TryGetValue(sceneName, out var sceneObjects))
        {
            if (sceneObjects.TryGetValue(uniqueID, out var componentDict))
            {
                if (componentDict.TryGetValue(componentName, out object state))
                    return state;
            }
        }
        return null;
    }

    public bool IsObjectComponentSaved(string uniqueID, string componentName, string sceneName = null)
    {
        return GetObjectComponentState(uniqueID, componentName, sceneName) != null;
    }
}

// =========================================
//            CHECKPOINT SAVE DATA
// =========================================
[System.Serializable]
public class PlayerCheckpointData
{
    public string sceneName;
    public Vector3 position;
    public Vector3 rotation;
    public float health;
}