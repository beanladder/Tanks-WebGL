using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;

public class Projectile : MonoBehaviour
{
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public GameObject TankHitAudio;
    public GameObject boomPrefab; // Prefab to instantiate when hitting a tank
    public GameObject hitwallPrefab; // Prefab to instantiate when hitting a wall/prop
    public GameObject smokeParticlePrefab; // Prefab to instantiate when hitting anything (for smoke grenade)
    public bool isSmokeGrenade; // Flag to indicate if this is a smoke grenade
    int DamageAmt;
    private void Update()
    {
        isSmokeGrenade = ProjectileLauncher.instance.isSmoke;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isSmokeGrenade)
        {
            HandleSmokeGrenadeCollision(collision);
        }
        else
        {
            HandleProjectileCollision(collision);
        }
    }

    private void HandleProjectileCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            StartCoroutine(PlayAudio("Boom"));
        }
        else if (collision.gameObject.CompareTag("Cylinder"))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            StartCoroutine(PlayAudio("Ricochet"));
        }
        else if (collision.gameObject.CompareTag("Tank"))
        {
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = Instantiate(boomPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(explosion, 2f);
            GameObject audioCont = Instantiate(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
            AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
            audioSrc.Play();
            Destroy(audioCont, 2f);
            DamageAmt = Random.Range(5, 14);
            collision.gameObject.GetComponent<TankInfo>().TakeDamage(DamageAmt);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = Instantiate(hitwallPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(explosion, 2f);
            GameObject audioCont = Instantiate(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
            AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
            audioSrc.Play();
            Destroy(audioCont, 2f);
            Destroy(gameObject);
        }
    }

    private void HandleSmokeGrenadeCollision(Collision collision)
    {
        // Instantiate the smoke particle system at the collision point
        GameObject smokeParticle =  Instantiate(smokeParticlePrefab, collision.contacts[0].point, Quaternion.identity);

        // Play the audio (if needed)
        StartCoroutine(PlayAudio("Boom"));

        // Destroy the smoke grenade after playing the audio
        Destroy(gameObject);
        Destroy(smokeParticle, 12f);
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
        else if (ID == "Ricochet")
        {
            int chance = Random.Range(0, Ricochet.Length);
            Ricochet[chance].Play();
        }
    }
}
