using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneIdentity : MonoBehaviour
{
    public bool isIntern;

    private void Start()
    {
        // On scene load, apply saved state
        SceneSwapManager.Instance.LoadState(gameObject, isIntern);
    }

    private void OnDestroy()
    {
        // When leaving a scene, save the state
        if (SceneSwapManager.Instance != null)
            SceneSwapManager.Instance.SaveState(gameObject, isIntern);
    }
}