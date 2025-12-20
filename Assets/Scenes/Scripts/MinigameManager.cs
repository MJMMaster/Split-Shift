using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("Bricks")]
    public int totalBricks;
    private int bricksDestroyed = 0;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TMPro.TextMeshProUGUI scoreText;

    private int score = 0;

    [Header("Scenes")]
    public string returnSceneName = "Intern";
    public int returnDelay = 2;

    [Header("Audio")]
    public AudioClip winClip;
    public AudioClip loseClip;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        UpdateScoreUI();
    }

    // =========================
    // BRICKS
    // =========================
    public void RegisterBrick()
    {
        totalBricks++;
    }

    public void BrickDestroyed()
    {
        if (gameEnded) return;

        bricksDestroyed++;
        score += 100;
        UpdateScoreUI();

        if (bricksDestroyed >= totalBricks)
        {
            WinGame();
        }
    }

    // =========================
    // GAME STATES
    // =========================
    private void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Minigame WON");

        if (winClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(winClip);

        if (winPanel != null)
            winPanel.SetActive(true);

        HeroAbilityManager.Instance?.UnlockDash();

        Time.timeScale = 0f;
        StartCoroutine(ReturnAfterDelay());
    }

    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Minigame LOST");

        if (loseClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(loseClip);

        if (losePanel != null)
            losePanel.SetActive(true);
    }

    // =========================
    // UI BUTTON FUNCTIONS
    // =========================
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(returnSceneName);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(returnDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(returnSceneName);
    }
}