using UnityEngine;

[System.Serializable]
public struct CheckpointData
{
    public string sceneName;
    public Vector3 position;
    public float health;
}

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public CheckpointData CurrentCheckpoint { get; private set; }
    public bool HasCheckpoint { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCheckpoint(Vector3 position, float health)
    {
        CurrentCheckpoint = new CheckpointData
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            position = position,
            health = health
        };

        HasCheckpoint = true;

        Debug.Log($"Checkpoint saved at {position} (HP = {health})");
    }

    public CheckpointData GetCheckpoint()
    {
        return CurrentCheckpoint;
    }
}