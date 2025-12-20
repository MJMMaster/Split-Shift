using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameUI : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitMinigame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Intern");
    }
}