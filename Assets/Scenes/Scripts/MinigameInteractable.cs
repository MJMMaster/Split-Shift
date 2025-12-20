using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameInteractable : InteractableBase, IInteractable
{
    [Header("Minigame Settings")]
    public string minigameSceneName = "BrickBreaker";
    public string returnSceneName = "Intern";

    public override void Interact()
    {
        // Tell the MinigameManager where to return
        MinigameReturnData.ReturnScene = returnSceneName;

        SceneManager.LoadScene(minigameSceneName);
    }
}