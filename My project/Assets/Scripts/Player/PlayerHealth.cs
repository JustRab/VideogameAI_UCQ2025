using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isImmortal = false;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public KeyCode toggleImmortalKey = KeyCode.I;

    private HordeManager hordeManager;

    void Start()
    {
        currentHealth = maxHealth;
        hordeManager = FindObjectOfType<HordeManager>();
        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleImmortalKey))
        {
            isImmortal = !isImmortal;
            healthText.color = isImmortal ? Color.yellow : Color.white;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isImmortal) return;

        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0f)
            hordeManager.OnPlayerDeath();
    }

    void UpdateHealthUI()
    {
        healthText.text = $"HP: {currentHealth:F0}/{maxHealth:F0}";
    }
}
