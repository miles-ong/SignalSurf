using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Settings")]
    [SerializeField] private bool showHealthPercentage = true;

    void Update()
    {
        if (player == null)
            return;

        UpdateHealthUI();
        UpdateScoreUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = player.CurrentHealth / player.MaxHealth;
        }

        if (healthText != null && showHealthPercentage)
        {
            float healthPercent = (player.CurrentHealth / player.MaxHealth) * 100f;
            healthText.text = $"{healthPercent:F0}%";
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {player.CurrentScore:F0}";
        }
    }
}
