using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NetworkProjectile : MonoBehaviour
{
    public int Damage;
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile collides with an object tagged as "Ground"

        if (collision.gameObject.CompareTag("Cylinder"))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            StartCoroutine(PlayAudio("Ricochet"));
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            

            // Disable the projectile renderer to make it invisible
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            // Destroy the projectile GameObject
            StartCoroutine(PlayAudio("Boom"));
            
        }
        
        
        
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        else if (ID == "Ricochet")
        { 
            int chance = Random.Range(0, Ricochet.Length);
            Ricochet[chance].Play();
        }
        
    }
}
