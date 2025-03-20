using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float rotationSpeed = 1000f; // Speed of spinning
    public GameObject deathEffect;

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Shuriken hit something: " + collision.name); // Check if this prints in the console

        if (collision.CompareTag("Boss"))
        {
            EnemyBoss enemyBoss = collision.GetComponent<EnemyBoss>();

            if (enemyBoss == null)
            {
                Debug.LogError("Shuriken hit Boss, but EnemyBoss script is missing!");
                return;
            }

            enemyBoss.TakeDamage(Random.Range(3, 10));
            Debug.Log("Shuriken hit Boss and dealt damage!"); // Debugging
            Destroy(gameObject);
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

       else if (collision.CompareTag("Enemy")) // Check if it hits an enemy
        {
            DroneHealth droneHealth = collision.GetComponent<DroneHealth>();
            if (droneHealth != null)
            {
                droneHealth.TakeDamage(3); // Apply damage
                Debug.Log("Shuriken hit Drone!");
            }
            DestroyShuriken();
        }
        
        else if (collision.CompareTag("Ground"))
        {
            Debug.Log("Shuriken hit the Ground!");
            DestroyShuriken();
        }
    }
   

    private void DestroyShuriken()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
