using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float rotationSpeed = 200f; // Speed of rotation
    public float bulletSpeed = 5f;     // Speed of the bullet

    private Rigidbody2D rb;
    public GameObject deathEffect;

    //public Random random;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * bulletSpeed; // Move bullet forward
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

         
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage if the player has a health script
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Random.Range(2, 10)); // Adjust damage as needed

                Debug.Log("nag tama sa tao");
            }


            Destroy(gameObject); // Destroy bullet on impact
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
        if (collision.CompareTag("Ground") )
        {
            Destroy(gameObject); // Destroy bullet if it hits the environments
            Debug.Log("nag tama sa Ground");
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

    }
    
}
