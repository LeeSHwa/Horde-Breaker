using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;


    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHP / maxHP;
        }
    }
}