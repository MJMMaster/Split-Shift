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
        "HeroHealth",
        "HeroPawn",
        "PlayerPawn",
    };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

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

                //  Do not autosave player
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

                if (excludedComponents.Contains(compName))
                    continue;

                if (sceneObjects.TryGetValue(so.UniqueID, out var compDict))
                {
                    if (compDict.TryGetValue(compName, out object savedData))
                    {
                        try
                        {
                            saveable.RestoreState(savedData);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Restore error on {so.UniqueID}: {e}");
                        }
                    }
                }
            }
        }

        Debug.Log($"[Autosave] Scene '{sceneName}' restored.");
    }

    // -----------------------------------------
    //            CHECKPOINT SYSTEM
    // -----------------------------------------
    public void SaveCheckpoint()
    {
        if (playerTransform == null)
            CachePlayerReferences();

        if (playerTransform == null || playerHealth == null)
            return;

        checkpoint = new PlayerCheckpointData
        {
            sceneName = SceneManager.GetActiveScene().name,
            position = playerTransform.position,
            rotation = playerTransform.eulerAngles,
            health = playerHealth.currentHealth
        };

        Debug.Log("[Checkpoint] Saved player data.");
    }

    public void LoadCheckpoint()
    {
        if (checkpoint == null)
        {
            Debug.LogWarning("[Checkpoint] No checkpoint to load!");
            return;
        }

        SceneManager.sceneLoaded -= OnCheckpointSceneLoaded;
        SceneManager.sceneLoaded += OnCheckpointSceneLoaded;

        SceneManager.LoadScene(checkpoint.sceneName);
    }

    private void OnCheckpointSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnCheckpointSceneLoaded;

        CachePlayerReferences();

        if (playerTransform == null || playerHealth == null)
            return;

        playerTransform.position = checkpoint.position;
        playerTransform.eulerAngles = checkpoint.rotation;

        playerHealth.Revive(checkpoint.health);

        Debug.Log("[Checkpoint] Player restored.");
    }

    // -----------------------------------------
    //        CROSS-SCENE HERO WALL REMOVAL
    // -----------------------------------------
    public void TriggerHeroWallRemoval(string wallID)
    {
        string heroScene = "Hero"; //  Change to your real hero scene name

        if (!allSceneStates.ContainsKey(heroScene))
            allSceneStates[heroScene] = new Dictionary<string, Dictionary<string, object>>();

        if (!allSceneStates[heroScene].ContainsKey(wallID))
            allSceneStates[heroScene][wallID] = new Dictionary<string, object>();

        allSceneStates[heroScene][wallID]["HeroRemovableWall"] =
            new HeroRemovableWall.WallSaveData { isActive = false };


        Debug.Log($"[Cross-Scene] Hero wall {wallID} removed.");
    }

    // =========================================
    //     CROSS-SCENE INTERN ITEM SPAWN
    // =========================================

    public void TriggerInternItemSpawn(string itemID)
    {
        string internScene = "Intern"; // must match scene name exactly

        if (!allSceneStates.ContainsKey(internScene))
            allSceneStates[internScene] = new Dictionary<string, Dictionary<string, object>>();

        if (!allSceneStates[internScene].ContainsKey(itemID))
            allSceneStates[internScene][itemID] = new Dictionary<string, object>();

        allSceneStates[internScene][itemID]["InternItemSpawner"] = true;

        Debug.Log($"[Cross-Scene] Intern item '{itemID}' queued for spawn.");
    }
    public T FindSaveableByID<T>(string id) where T : MonoBehaviour
    {
        var all = FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var obj in all)
        {
            if (obj is T t)
            {
                var saveable = obj.GetComponent<SaveableObject>();
                if (saveable != null && saveable.UniqueID == id)
                    return t;
            }
        }
        return null;
    }

    public bool IsObjectComponentSaved(string uniqueID, string componentName, string sceneName = null)
    {
        return GetObjectComponentState(uniqueID, componentName, sceneName) != null;
    }

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