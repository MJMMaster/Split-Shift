using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance;

    public SceneState internState;
    public SceneState heroState;

    private string internScene = "Intern";
    private string heroScene = "Hero";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        // TEMPORARY TEST SWITCH KEY
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Swap();
        }
    }

    public void SaveState(GameObject character, bool isIntern)
    {
        var rb = character.GetComponent<Rigidbody>();

        SceneState s = new SceneState(
            character.transform.position,
            character.transform.rotation,
            rb != null ? rb.linearVelocity : Vector3.zero
        );

        if (isIntern) internState = s;
        else heroState = s;
    }

    public void LoadState(GameObject character, bool isIntern)
    {
        SceneState s = isIntern ? internState : heroState;
        if (s == null) return;

        var rb = character.GetComponent<Rigidbody>();

        character.transform.position = s.position;
        character.transform.rotation = s.rotation;

        if (rb != null) rb.linearVelocity = s.velocity;
    }

    private void Swap()
    {
        // Save the current scene state
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveSceneState();
        }

        string current = SceneManager.GetActiveScene().name;

        if (current == internScene)
            SceneManager.LoadScene(heroScene);
        else
            SceneManager.LoadScene(internScene);
    }
}