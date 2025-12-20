using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip sceneMusic;

    private void Start()
    {
        AudioManager.Instance?.PlayMusic(sceneMusic);
    }
}