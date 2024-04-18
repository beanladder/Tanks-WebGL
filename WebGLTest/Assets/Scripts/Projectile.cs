using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Projectile : MonoBehaviour
{
    
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public int DamageAmt = 5;

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
        
        if(collision.gameObject.CompareTag("Tank")){
            Debug.Log("Hit Tank");
            gameObject.GetComponent<TrailRenderer>().enabled = false; 
            gameObject.GetComponent<Renderer>().enabled = false;
            collision.gameObject.GetComponent<TankInfo>().TakeDamage(DamageAmt);
            StartCoroutine(InitiateDamage());
            

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

    public IEnumerator InitiateDamage()
    {
        Debug.Log("Damage Confirmed");
        Ricochet[0].Play();
        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        
    }
    
}