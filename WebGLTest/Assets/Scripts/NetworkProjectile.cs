// using System.Collections;
// using System.Collections.Generic;
// using ExitGames.Client.Photon.StructWrapping;
// using Photon.Pun;
// using UnityEngine;
// using UnityEngine.Animations;

// public class NetworkProjectile : MonoBehaviour
// {
//     public AudioSource Boom;
//     public AudioSource[] Ricochet;
//     public GameObject TankHitAudio;
//     public GameObject boomPrefab; // Prefab to instantiate when hitting a tank
//     public GameObject hitwallPrefab; // Prefab to instantiate when hitting a wall/prop
//     int DamageAmt;

//     private void OnCollisionEnter(Collision collision)
//     {
//         HandleProjectileCollision(collision);
//     }

//     private void HandleProjectileCollision(Collision collision)
//     {
//         // if (collision.gameObject.CompareTag("Ground"))
//         // {
//         //     gameObject.GetComponent<Renderer>().enabled = false;
//         //     gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
//         //     StartCoroutine(PlayAudio("Boom"));
//         // }
//         if (collision.gameObject.CompareTag("Cylinder"))
//         {
//             gameObject.GetComponent<Renderer>().enabled = true;
//             StartCoroutine(PlayAudio("Ricochet"));
//         }
//         else if (collision.gameObject.CompareTag("Tank"))
//         {
//             gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
//             gameObject.GetComponent<Renderer>().enabled = false;
//             GameObject explosion = Instantiate(boomPrefab, collision.contacts[0].point, Quaternion.identity);
//             Destroy(explosion, 2f);
//             GameObject audioCont = Instantiate(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
//             AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
//             audioSrc.Play();
//             Destroy(audioCont, 2f);
//             DamageAmt = Random.Range(5, 14);
//             PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
//             if(targetView!=null && targetView.IsMine){
//                 targetView.RPC("TakeDamage",RpcTarget.All,DamageAmt);
//             }
//             //collision.gameObject.GetComponent<TankInfo>().TakeDamage(DamageAmt);
//             Destroy(gameObject);
//         }
//         else if (collision.gameObject.CompareTag("Wall") ||  collision.gameObject.CompareTag("Ground"))
//         {
//             gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
//             gameObject.GetComponent<Renderer>().enabled = false;
//             GameObject explosion = Instantiate(hitwallPrefab, collision.contacts[0].point, Quaternion.identity);
//             Destroy(explosion, 2f);
//             GameObject audioCont = Instantiate(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
//             AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
//             audioSrc.Play();
//             Destroy(audioCont, 2f);
//             Destroy(gameObject);
//         }
//     }

//     public IEnumerator PlayAudio(string ID)
//     {
//         if (ID == "Boom")
//         {
//             Boom.Play();
//             yield return new WaitForSeconds(0.5f);
//             Destroy(gameObject);
//         }
//         else if (ID == "Ricochet")
//         {
//             int chance = Random.Range(0, Ricochet.Length);
//             Ricochet[chance].Play();
//         }
//     }
// }
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;

public class NetworkProjectile : MonoBehaviourPunCallbacks
{
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public GameObject TankHitAudio;
    public GameObject boomPrefab;
    public GameObject hitwallPrefab;
    private int damageAmt;
    public float maxLifetime = 5f;
    private float lifetimeTimer;
    private bool hasHitFinalTarget = false;
    private int shooterViewID;
    

    public void InitializeProjectile(int viewID)
    {
        shooterViewID = viewID;
        lifetimeTimer = 0f;
        hasHitFinalTarget = false;
        EnableRenderers(true);
        Debug.Log($"Projectile initialized: {gameObject.name} at {transform.position}");
    }

    private void Update()
    {
        if (!hasHitFinalTarget)
        {
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= maxLifetime)
            {
                DisableProjectile();
            }
            Debug.DrawRay(transform.position, GetComponent<Rigidbody>().velocity.normalized * 5f, Color.red);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile collided: {gameObject.name} with {collision.gameObject.name} at {collision.contacts[0].point}");
        HandleProjectileCollision(collision);
    }

    private void HandleProjectileCollision(Collision collision)
    {
        if (hasHitFinalTarget) return;

        if (collision.gameObject.CompareTag("Cylinder"))
        {
            HandleCylinderCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Tank") ||
                 collision.gameObject.CompareTag("Wall") ||
                 collision.gameObject.CompareTag("Ground"))
        {
            hasHitFinalTarget = true;
            DisableProjectile(); // Immediately disable the projectile
            HandleFinalCollision(collision);
        }
    }

    private void HandleCylinderCollision(Collision collision)
    {
        EnableRenderers(true);
        StartCoroutine(PlayAudio("Ricochet"));

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 reflectedVelocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            rb.velocity = reflectedVelocity * 0.8f;
        }
    }

    private void HandleFinalCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tank"))
        {
            HandleTankCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            HandleWallOrGroundCollision(collision);
        }

        StartCoroutine(DelayedCleanup());
    }

    private void HandleTankCollision(Collision collision)
    {
        GameObject explosion = Instantiate(boomPrefab, collision.contacts[0].point, Quaternion.identity);
        Destroy(explosion, 2f);
        PlayAudioAtPoint(TankHitAudio, collision.contacts[0].point);
        damageAmt = Random.Range(4, 9);

        PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
        if (targetView != null)
        {
            targetView.RPC("TakeDamage", RpcTarget.All, damageAmt, shooterViewID);
        }
    }

    private void HandleWallOrGroundCollision(Collision collision)
    {
        GameObject explosion = Instantiate(hitwallPrefab, collision.contacts[0].point, Quaternion.identity);
        Destroy(explosion, 2f);
        PlayAudioAtPoint(TankHitAudio, collision.contacts[0].point);
    }

    private void PlayAudioAtPoint(GameObject audioPrefab, Vector3 position)
    {
        GameObject audioCont = Instantiate(audioPrefab, position, Quaternion.identity);
        AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
        audioSrc.Play();
        Destroy(audioCont, 2f);
    }

    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(2f);
        ReturnToPool();
    }

    private void DisableProjectile()
    {
        
        EnableRenderers(false);
        GetComponent<Collider>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    private void EnableRenderers(bool enable)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = enable;

        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        if (trail != null) trail.enabled = enable;

        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();
        //if(particle != null) 
    }

    private void ReturnToPool()
    {
        Debug.Log($"Projectile returned to pool: {gameObject.name}");
        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetProjectile()
    {
        hasHitFinalTarget = false;
        EnableRenderers(true);
        GetComponent<Collider>().enabled = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        lifetimeTimer = 0f;
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(0.5f);
        }
        else if (ID == "Ricochet")
        {
            int chance = Random.Range(0, Ricochet.Length);
            Ricochet[chance].Play();
        }
    }
}
