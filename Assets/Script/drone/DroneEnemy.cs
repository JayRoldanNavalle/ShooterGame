using UnityEngine;
using UnityEngine.UIElements;

public class DroneEnemy : MonoBehaviour
{
    [Header("Drone Movement")]
    private float speed ;          // Speed of the drone
    public float stopDistance = 5f;   // Distance to stop moving towards the player
    public Transform[] patrolPoints;  // Waypoints for patrolling
    private int currentPatrolIndex = 0;

    [Header("Shooting")]
    public GameObject bulletPrefab;   // Assign your bullet prefab in the Inspector
    public Transform firePoint;       // Empty GameObject where bullets spawn

    public Transform rayCastPoint;
    public float fireRate = 1.5f;     // Time between shots
    public float bulletSpeed = 10f;   // Speed of bullets
    private float fireCooldown = 0f;

    private Transform player;
    
    private Animator animator;
    private bool playerBlocked = false; // True if a wall is between player and enemy

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Finds the player by tag
        animator = GetComponent<Animator>(); // Get the Animator component

        speed = Random.Range(3, 6);
       
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer);  // Enemies can pass each other
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer);



        //para mag patrol tulos


        Patrol(); 
        // True if there's a wall
        playerBlocked = true;
    }

    private void Update()
    {
        if (player == null) return; // Stop if no player is found

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
        /*else
        { 
         playerBlocked = true;
        }*/
       



       /* if (playerBlocked == false && distanceToPlayer > stopDistance) 
        {
           Patrol();
        }*/



       

       

        if (playerBlocked || distanceToPlayer > stopDistance)//
        {
            
            Patrol(); // If the player is blocked by a wall, patrol instead
        }
        else
        {
           
                 ChaseAndShoot(distanceToPlayer);
          
                 transform.right = direction; // Rotate to face the player
            
           
        }

        // Floating movement effect
        transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.5f * Time.deltaTime, 0);



        if (fireCooldown > 0.1)
        {
            animator.SetBool("isShooting", false);
        }
        else
        {
            animator.SetBool("isShooting", true);
        }
    }

    private void ChaseAndShoot(float distanceToPlayer)
    {
        if (distanceToPlayer > stopDistance)
        {
           
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f && distanceToPlayer <= stopDistance && !playerBlocked)
        {
            Shoot();
            fireCooldown = fireRate; // Reset cooldown
        }

       
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        // **Rotate toward patrol point**
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.right = direction;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        
    }

    private void Shoot()
    {
        AudioManager.Instance.PlaySFX("laser");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed; // Shoot bullet in the direction of firePoint
        Destroy(bullet, 3f); // Destroy bullet after 3 seconds to save memory
    }


  
}
