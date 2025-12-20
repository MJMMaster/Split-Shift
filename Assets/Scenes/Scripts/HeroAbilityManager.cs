using UnityEngine;

public class HeroAbilityManager : MonoBehaviour
{
    public static HeroAbilityManager Instance;
    public bool showDashUnlockMessage = false;

    [Header("Hero Abilities")]
    public bool dashUnlocked = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockDash()
    {
        dashUnlocked = true;
        showDashUnlockMessage = true;
        Debug.Log("Dash ability unlocked!");
    }
}