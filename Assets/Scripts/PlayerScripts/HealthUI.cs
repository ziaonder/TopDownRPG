using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmPro;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChange += ChangeHealthTextUI;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChange -= ChangeHealthTextUI;
    }

    public void ChangeHealthTextUI(int health)
    {
        tmPro.text = health.ToString();
    }
}
