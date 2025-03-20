using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [Header("Boss Movement")]
    public float speed = 3f;
    public float stopDistance = 6f;
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform firePoint;
    public Transform rayCastPoint;
    public float fireRate = 1f;
    public float missileRate = 3f;
    public float bulletSpeed = 10f;
    public float missileSpeed = 5f;
    public float knockbackForce = 2f;

    [Header("Attack Pattern")]
    public int bulletShotsBeforeMissile = 3;
    private float fireCooldown = 0f;
    private float missileCooldown = 0f;
    private int bulletCount = 0;

    [Header("Boss Health & Phases")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isPhaseTwo = false;  // Tracks if the boss is in Phase 2

    public Transform player;
    private Animator animator;
    private bool playerBlocked = false;

    private void Start()
    {
        
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        Patrol();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distanceToPlayer < stopDistance)
        {
            // Check if a wall is between the player and enemy
            RaycastHit2D hit = Physics2D.Raycast(rayCastPoint.position, direction, distanceToPlayer, LayerMask.GetMask("Ground"));


            // Debugging: Draw a ray in the scene view
            Debug.DrawRay(rayCastPoint.position, direction * distanceToPlayer, hit.collider == null ? Color.green : Color.red);

            playerBlocked = hit.collider != null; // True if there's a wall
        }

        if (playerBlocked || distanceToPlayer > stopDistance)
        {
            Patrol();
        }
        else
        {
            ChaseAndShoot(distanceToPlayer);
            transform.right = direction;
        }

        // Floating movement effect
        transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.5f * Time.deltaTime, 0);
    }

    private void ChaseAndShoot(float distanceToPlayer)
    {
        if (distanceToPlayer > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

        fireCooldown -= Time.deltaTime;
        missileCooldown -= Time.deltaTime;

        // PHASE 2: Faster missiles & spread bullets
        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (fireCooldown <= 0f && bulletCount < bulletShotsBeforeMissile)
        {
            if (isPhaseTwo)
            {
                SpreadShot(); // Shoot in multiple directions in Phase 2
            }
            else
            {
                ShootBullet(); // Normal shooting in Phase 1
            }

            fireCooldown = fireRate;
            bulletCount++;
        }
        else if (bulletCount >= bulletShotsBeforeMissile && missileCooldown <= 0f)
        {
            ShootMissile();
            missileCooldown = missileRate;
            bulletCount = 0;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.right = direction;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
        Destroy(bullet, 3f);
    }

    private void SpreadShot()
    {
        float[] angles = { -20f,-10 , 0f ,10 ,20f };  // Three directions: left, center, right
        foreach (float angle in angles)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            Vector3 rotation = firePoint.eulerAngles;
            rotation.z += angle;
            bullet.transform.eulerAngles = rotation;

            rb.velocity = bullet.transform.right * bulletSpeed;
            Destroy(bullet, 3f);
        }
    }

    private void ShootMissile()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * missileSpeed;

        rb.AddForce(-transform.right * knockbackForce, ForceMode2D.Impulse);

        Destroy(missile, 5f);
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        bulletSpeed *= 1.2f;   // Increase bullet speed
        missileSpeed *= 1.5f;  // Faster missiles
        fireRate *= 0.8f;      // Shoots faster
        Debug.Log("Boss has entered Phase 2!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss took " + damage + " damage!"); // Debugging
        if (currentHealth <= 0)
        {
            Die(); // Call death function
        }
    }


    private void Die()
    {
        Debug.Log("Boss Defeated!");
        Destroy(gameObject);
    }
}
