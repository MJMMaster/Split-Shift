using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // set your main menu scene name in inspector

    // ------------------------------
    // Called by the "Main Menu" button
    // ------------------------------
    public void GoToMainMenu()
    {
        Debug.Log("[CreditsManager] Returning to Main Menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ------------------------------
    // Called by the "Quit Game" button
    // ------------------------------
    public void QuitGame()
    {
        Debug.Log("[CreditsManager] Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stop play mode in editor
#else
        Application.Quit(); // quit build
#endif
    }
}