using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;

public class NetworkSecondary : MonoBehaviour
{
    public AudioSource Boom;
    public GameObject smokeParticlePrefab; // Prefab to instantiate when hitting anything (for smoke grenade)

    private void OnCollisionEnter(Collision collision)
    {
        HandleSmokeGrenadeCollision(collision);
    }

    private void HandleSmokeGrenadeCollision(Collision collision)
    {
        // Instantiate the smoke particle system at the collision point
        GameObject smokeParticle = Instantiate(smokeParticlePrefab, collision.contacts[0].point, Quaternion.identity);

        // Play the audio (if needed)
        StartCoroutine(PlayAudio("Boom"));

        // Destroy the smoke grenade after playing the audio
        Destroy(gameObject);
        Destroy(smokeParticle, 17f);
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}
