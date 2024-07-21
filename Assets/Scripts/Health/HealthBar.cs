using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image healthBarTotal;
    [SerializeField] private Image healthBarCurrent;
    void Start()
    {
        healthBarTotal.fillAmount = playerHealth.currentHealth / 10;
    }


    void Update()
    {
        healthBarCurrent.fillAmount = playerHealth.currentHealth / 10;
    }
}
