using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI")]
    public Slider healthBar; // Assign a UI Slider in Inspector

    public GameObject deathEffect;

    private void Start()
    {
        Time.timeScale = 1;
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
        Debug.Log($"Buhay {currentHealth -= damage}");
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevents negative health

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
            
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
        Debug.Log("Player has died!");

        // Disable movement & actions
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<ShurikenShooter>().enabled = false;

        // Play death animation if you have one
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }




        StartCoroutine(Wait());


        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, transform.rotation);
        
    }
    IEnumerator Wait()
    {
       
        yield return new WaitForSeconds(3f);
        Time.timeScale = 0;
    }
   
}
