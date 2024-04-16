using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile collides with an object tagged as "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the projectile GameObject
            Destroy(gameObject);
        }
        
    }
}
