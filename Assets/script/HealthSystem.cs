using UnityEngine;
using UnityEngine.UI; // Needed for UI slider/health bar

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI (Optional)")]
    public Slider healthBar;  // Drag your UI Slider here in Inspector
    public Text healthText;   // Optional text for displaying HP

    [Header("Death Settings")]
    public GameObject deathCanvas; // UI Canvas when dead

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
    }


    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        // Example: disable player movement
        // GetComponent<PlayerMove>().enabled = false;

        // Example: reload scene after delay
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


   
}
