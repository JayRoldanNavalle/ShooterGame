using System.Collections;
using UnityEngine;

public class ShurikenShooter : MonoBehaviour
{
    [Header("Shuriken Settings")]
    public GameObject shurikenPrefab;  // Assign your Shuriken prefab in the Inspector
    public Transform firePoint;        // Empty GameObject where the shuriken spawns
    public float shurikenSpeed = 15f;  // Speed of the thrown shuriken
    public float fireRate = 0.5f;      // Cooldown between throws
    public int maxShurikens = 5;       // Maximum number of shurikens before reloading

    private bool canThrow = true;
    private int currentShurikens;
    private Animator animator;
    private PlayerMovement playerMovement;

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
        playerMovement = FindObjectOfType<PlayerMovement>(); // Get PlayerMovement script
        currentShurikens = maxShurikens;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canThrow && currentShurikens > 0) //Click to throw
        {
            ThrowShuriken();
        }
    }

    private void ThrowShuriken()
    {
        canThrow = false;
        currentShurikens--;

        if (animator != null)
            animator.SetTrigger("Shoot");


        AudioManager.Instance.PlaySFX("Shuriken");

        // Instantiate the shuriken at the fire point
        GameObject shuriken = Instantiate(shurikenPrefab, firePoint.position, Quaternion.identity);

        // Get the Rigidbody2D and set its velocity
        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();

        // Get Mouse Position in World Coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate Direction to Mouse
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        // Set Velocity
        rb.velocity = direction * shurikenSpeed;

        // Rotate Shuriken to Face Direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shuriken.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Destroy shuriken after a few seconds
        Destroy(shuriken, 3f);

        // Start cooldown
        StartCoroutine(ThrowCooldown());
    }


    private IEnumerator ThrowCooldown()
    {
        
        yield return new WaitForSeconds(fireRate);
        animator.ResetTrigger("Shoot");
        canThrow = true;
    }

    public void ReloadShurikens()
    {
        currentShurikens = maxShurikens;
    }
}
