using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Loads the gameplay scene where the Intern starts
    public void StartGame()
    {
        SceneManager.LoadScene("Intern");
    }

    // Loads your Settings screen
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    // Loads your Credits screen
    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    // Optional: Quit button
    public void QuitGame()
    {
        Debug.Log("Quit Game pressed!");
        Application.Quit(); // Will quit in a build, not in the editor
    }
}