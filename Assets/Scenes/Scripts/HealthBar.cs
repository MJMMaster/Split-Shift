using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    private Health target;

    public void Initialize(Health health)
    {
        target = health;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float percent = target.currentHealth / target.maxHealth;
        fillImage.fillAmount = percent;

        // Optional: hide when full
        fillImage.transform.parent.gameObject.SetActive(percent < 1f);
    }
}