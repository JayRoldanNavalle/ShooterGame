using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float rotationSpeed = 200f;  // Speed of rotation
    public float bulletSpeed = 5f;      // Speed of the missile
    public float explosionDelay = 0.2f; // Delay before destroying the missile
    public float knockbackForce = 5f;   // Force applied to player on impact

    private Rigidbody2D rb;
    public GameObject deathEffect;

    private Transform player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Move towards player
        transform.position = Vector2.MoveTowards(transform.position, player.position, bulletSpeed * Time.deltaTime);

        // Smoothly rotate towards player
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Random.Range(2, 10)); // Adjust damage as needed
                Debug.Log("Missile hit the player!");
            }

            // Apply knockback
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            StartCoroutine(Explode());
        }

        if (collision.CompareTag("Ground"))
        {
            Debug.Log("Missile hit the ground!");
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        yield return new WaitForSeconds(explosionDelay);
        Destroy(gameObject); // Destroy missile after delay
    }
}
