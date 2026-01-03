using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Image fillImage;

    void Update()
    {
        if (!playerHealth) return;

        fillImage.fillAmount =
            (float)playerHealth.currentHealth / playerHealth.maxHealth;
    }
}
