using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public GameObject gameOverScreen;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (gameOverScreen != null)
                DontDestroyOnLoad(gameOverScreen.transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        // Reset time scale and Game Over state
        Time.timeScale = 1f;
        isGameOver = false;

        // Hide Game Over screen immediately
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        // Register callback BEFORE reload
        SceneManager.sceneLoaded += OnSceneReloaded;

        // Load the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneReloaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneReloaded;

        if (!CheckpointManager.Instance.HasCheckpoint)
        {
            Debug.LogWarning("No checkpoint to restore.");
            return;
        }

        // Start coroutine to restore player after end of frame
        StartCoroutine(RestorePlayerAtCheckpoint());
    }

    private IEnumerator RestorePlayerAtCheckpoint()
    {
        yield return new WaitForEndOfFrame(); // wait for scene to fully initialize

        CheckpointData data = CheckpointManager.Instance.GetCheckpoint();

        HeroHealth hp = Object.FindFirstObjectByType<HeroHealth>();
        if (hp == null)
        {
            Debug.LogError("Could not find HeroHealth after reload!");
            yield break;
        }

        Transform player = hp.transform;

        // Restore health and revive
        hp.Revive(data.health);

        // Move player to checkpoint position
        player.position = data.position;

        // Re-enable movement
        HeroPawn move = player.GetComponent<HeroPawn>();
        if (move != null) move.enabled = true;

        Debug.Log($"[Checkpoint] Player restored at {data.position} with HP={data.health}");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}