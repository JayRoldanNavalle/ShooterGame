using UnityEngine;
using UnityEngine.UI;

public class DroneHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

   

    [Header("UI")]
    public Slider healthBar; // Assign a UI Slider in Inspector

    public GameObject deathEffect;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Buhay Ng Drone {currentHealth}");
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevents negative health

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();

            KillToNextLvl killToNextLvl = GameObject.Find("KillToNextLvl").GetComponent<KillToNextLvl>();

            if (killToNextLvl != null)
            {
                killToNextLvl.AddKill(1);
                Debug.Log("Kill added successfully.");
            }
            else
            {
                Debug.LogError("KillToNextLvl component is missing on the GameObject!");
            }
        }
    }



    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevents overhealing

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    private void Die()
    {
        isDead = true;
        
        Debug.Log("Drone has died!");

        Destroy(gameObject);
        
        Instantiate(deathEffect, transform.position, transform.rotation);
        
    }

   
}
