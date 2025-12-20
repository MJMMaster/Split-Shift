using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;

    [Header("Color Settings")]
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("Optional Boss Target")]
    public BossController bossTarget;

    private Health healthTarget;

    // =========================
    // INITIALIZATION
    // =========================
    public void Initialize(Health health)
    {
        healthTarget = health;
        bossTarget = null;
    }

    public void InitializeBoss(BossController boss)
    {
        bossTarget = boss;
        healthTarget = null;
    }

    // =========================
    // UPDATE
    // =========================
    private void LateUpdate()
    {
        float current;
        float max;

        if (bossTarget != null)
        {
            current = bossTarget.CurrentHealth;
            max = bossTarget.MaxHealth;
        }
        else if (healthTarget != null)
        {
            current = healthTarget.currentHealth;
            max = healthTarget.maxHealth;
        }
        else
        {
            return;
        }

        if (max <= 0f) return;

        float percent = Mathf.Clamp01(current / max);

        fillImage.fillAmount = percent;
        UpdateColor(percent);

        // Optional: hide the bar when at full health for normal enemies
        if (healthTarget != null)
        {
            fillImage.transform.parent.gameObject.SetActive(percent < 1f);
        }
        else
        {
            // For bosses, always show the bar
            fillImage.transform.parent.gameObject.SetActive(true);
        }
    }

    // =========================
    // COLOR LOGIC
    // =========================
    private void UpdateColor(float percent)
    {
        if (percent > 0.6f)
        {
            fillImage.color = Color.Lerp(midHealthColor, fullHealthColor, (percent - 0.6f) / 0.4f);
        }
        else
        {
            fillImage.color = Color.Lerp(lowHealthColor, midHealthColor, percent / 0.6f);
        }
    }
}