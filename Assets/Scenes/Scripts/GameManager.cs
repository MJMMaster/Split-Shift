using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Stores scene objects' states: scene name -> (uniqueID -> object state)
    private Dictionary<string, Dictionary<string, object>> allSceneStates = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestoreSceneState(scene.name);
    }

    /// <summary>
    /// Saves all ISaveable objects in the current scene.
    /// </summary>
    public void SaveSceneState()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        var sceneObjects = new Dictionary<string, object>();

        var allMono = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,  // Include inactive objects
            FindObjectsSortMode.None      // No sorting needed
        );
        var saveables = allMono.OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            if (saveable is MonoBehaviour mb)
            {
                SaveableObject so = mb.GetComponent<SaveableObject>();
                if (so == null) continue;

                sceneObjects[so.UniqueID] = saveable.CaptureState();
            }
        }

        allSceneStates[sceneName] = sceneObjects;
        Debug.Log($"Scene '{sceneName}' saved with {sceneObjects.Count} objects!");
    }

    /// <summary>
    /// Restores all ISaveable objects in the current scene.
    /// </summary>
    public void RestoreSceneState()
    {
        RestoreSceneState(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Restores all ISaveable objects in the specified scene.
    /// </summary>
    public void RestoreSceneState(string sceneName)
    {
        if (!allSceneStates.TryGetValue(sceneName, out var sceneObjects)) return;

        var allMono = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include, // Include inactive objects to restore collectibles
            FindObjectsSortMode.None
        );
        var saveables = allMono.OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            if (saveable is MonoBehaviour mb)
            {
                SaveableObject so = mb.GetComponent<SaveableObject>();
                if (so == null) continue;

                if (sceneObjects.TryGetValue(so.UniqueID, out object savedData))
                {
                    saveable.RestoreState(savedData);
                }
            }
        }

        Debug.Log($"Scene '{sceneName}' restored with {sceneObjects.Count} objects!");
    }

    /// <summary>
    /// Returns the saved state dictionary for the given scene.
    /// Useful for objects to check if they’ve been saved already.
    /// </summary>
    public Dictionary<string, object> GetSceneState(string sceneName = null)
    {
        sceneName ??= SceneManager.GetActiveScene().name;
        if (allSceneStates.TryGetValue(sceneName, out var sceneObjects))
            return sceneObjects;

        return new Dictionary<string, object>();
    }

    /// <summary>
    /// Checks if a specific object (by unique ID) has been saved in this scene.
    /// </summary>
    public bool IsObjectSaved(string uniqueID, string sceneName = null)
    {
        var sceneObjects = GetSceneState(sceneName);
        return sceneObjects.ContainsKey(uniqueID);
    }
}