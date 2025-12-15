using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyKeyToStart : MonoBehaviour
{
    void Update()
    {
        // Detect *any* key or button press
        if (Input.anyKeyDown)
        {
            LoadMenu();
        }
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}