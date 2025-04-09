using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hulog : MonoBehaviour
{
    // Start is called before the first frame update
   

  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage if the player has a health script
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(100); // Adjust damage as needed

               
            }


        }

    }
}
