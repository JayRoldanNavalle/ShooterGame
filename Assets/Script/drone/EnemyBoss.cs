using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss : MonoBehaviour
{
    [Header("Boss Movement")]
    public float speed = 15f;
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private int shotsFired = 0;
    private bool isMovingToNextPoint = false;
    public float patrolSpeed = 15f;
    public float pointReachedThreshold = 0.1f;


    [Header("Shooting")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;  // Change to 5 or 3 seconds for cooldown between shots
    public float missileRate = 3f;
    public float bulletSpeed = 10f;
    public float missileSpeed = 5f;
    public float knockbackForce = 2f;

    [Header("Attack Pattern")]
    public int bulletShotsBeforeMissile = 3;
    private float fireCooldown = 2f;
    private float missileCooldown = 0f;
    private int bulletCount = 0;

    [Header("Boss Health & Phases")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isPhaseTwo = false;  // Tracks if the boss is in Phase 2

    [Header("UI")]
    public Slider healthBar; // Assign a UI Slider in Inspector


    public Transform player; // Drag the player here in the Inspector
    public float detectionRange = 10f; // Set how close the player must be to shoot
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        Patrol(); // Start patrolling at the beginning

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime; // Countdown for the shooting cooldown
        missileCooldown -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Face the player before shooting
        FacePlayer();


        if (isMovingToNextPoint)
        {
            MoveToNextPoint();
            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            if (fireCooldown <= 0f)
            {
                Shoot();
                shotsFired++;

                if (shotsFired >= 7)
                {
                    shotsFired = 0;
                    isMovingToNextPoint = true;
                }

                fireCooldown = fireRate;
            }
        }
        else
        {
            Patrol(); // Optional fallback
        }

        fireCooldown -= Time.deltaTime;
        // Floating movement effect
        transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.5f * Time.deltaTime, 0);
    }

    private void Shoot()
    {

       
        // PHASE 2: Faster missiles & spread bullets
        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (bulletCount < bulletShotsBeforeMissile)
        {
            if (isPhaseTwo)
            {
                SpreadShot(); // Shoot in multiple directions in Phase 2
            }
            else
            {
                ShootBullet(); // Normal shooting in Phase 1
            }
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
        /*Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.right = direction;
*/
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ShootBullet()
    {
        AudioManager.Instance.PlaySFX("laser");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
        Destroy(bullet, 3f);
    }

    private void SpreadShot()
    {
        AudioManager.Instance.PlaySFX("laser");
        float[] angles = { -20f, -10f, 0f, 10f, 20f };  // Three directions: left, center, right
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
        AudioManager.Instance.PlaySFX("laser");
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
        
       

        Debug.Log("Boss has entered Phase 2!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss took " + damage + " damage!"); // Debugging

        if (healthBar != null)
            healthBar.value = currentHealth;

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

    private void FacePlayer()
    {
        if (player == null) return;

        // Make the boss face the player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.right = direction;  // Update the rotation to face the player
    }

    void MoveToNextPoint()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, targetPoint.position);
        if (distance < pointReachedThreshold)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            isMovingToNextPoint = false;
        }
    }

}
