using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Navigation")]
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // Initialize slider values
        musicSlider.value = AudioManager.Instance.musicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;

        // Hook slider events
        musicSlider.onValueChanged.AddListener(SetMusic);
        sfxSlider.onValueChanged.AddListener(SetSFX);
    }

    private void SetMusic(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void SetSFX(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    // =========================
    // BUTTON CALLBACK
    // =========================
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
