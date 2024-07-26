using System.Collections;
using Photon.Pun;
using UnityEngine;

public class NetworkEMP : MonoBehaviour
{
    public AudioSource Boom;
    public GameObject empEffectPrefab; // Prefab for visual effect when EMP explodes
    public float explosionRadius = 10f;
    public float empDuration = 4f;
    public LayerMask tankLayer;

    private void OnCollisionEnter(Collision collision)
    {
        HandleEMPCollision(collision);
    }

    private void HandleEMPCollision(Collision collision)
    {
        // Instantiate the EMP effect at the collision point
        GameObject empEffect = Instantiate(empEffectPrefab, collision.contacts[0].point, Quaternion.identity);

        // Apply the EMP effect to nearby tanks
        Explode();

        // Play the audio
        StartCoroutine(PlayAudio("Boom"));

        // Destroy the EMP grenade after playing the audio
        Destroy(gameObject);
        Destroy(empEffect, 17f);
    }

    void Explode()
    {
        Collider[] tanksInRange = Physics.OverlapSphere(transform.position, explosionRadius, tankLayer);

        foreach (Collider tank in tanksInRange)
        {
            NetworkProjectileLauncher projectileLauncher = tank.GetComponent<NetworkProjectileLauncher>();
            NetworkSquareMovement squareMovement = tank.GetComponent<NetworkSquareMovement>();
            if (projectileLauncher != null && squareMovement != null)
            {
                StartCoroutine(ApplyEMPEffect(projectileLauncher, squareMovement));
            }
        }
    }

    IEnumerator ApplyEMPEffect(NetworkProjectileLauncher projectileLauncher, NetworkSquareMovement squareMovement)
    {
        projectileLauncher.enabled = false;
        squareMovement.enabled = false;
        yield return new WaitForSeconds(empDuration);
        projectileLauncher.enabled = true;
        squareMovement.enabled = true;
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
