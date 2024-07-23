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
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Animations;

public class NetworkProjectile : MonoBehaviour
{
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public GameObject TankHitAudio;
    public GameObject boomPrefab; // Prefab to instantiate when hitting a tank
    public GameObject hitwallPrefab; // Prefab to instantiate when hitting a wall/prop
    public GameObject test;
    PhotonView view;
    private int damageAmt;
    

    void Start(){
        view = GetComponent<PhotonView>();
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        HandleProjectileCollision(collision);
    }

    public void GetOwnerID(int ID)
    {
        view.ViewID = ID;
    }

    private void HandleProjectileCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cylinder"))
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
            damageAmt = Random.Range(4, 9);
            Vector3 impactPosition = collision.contacts[0].point;
            float impulseForce = damageAmt / 5f; // Adjust as needed
            Vector3 impulseDirection = (impactPosition - transform.position).normalized;
            PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
        
        if (targetView != null)
        {
            // Get the owner name of the projectile
            string shooterName = view.Owner.NickName;

            // Get the name of the hit tank
            string hitTankName = targetView.Owner.NickName;

            // Log the collision information
            Debug.Log($"Projectile owned by {shooterName} hit tank owned by {hitTankName}");

            // Existing code for damage and camera shake...
            targetView.RPC("TakeDamage", RpcTarget.All, damageAmt);
            targetView.RPC("ShakeCamera", RpcTarget.All, impactPosition, impulseDirection, impulseForce);
        }
            //collision.gameObject.GetComponent<TankInfo>().TakeDamage(DamageAmt);
            //Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = Instantiate(hitwallPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(explosion, 2f);
            GameObject audioCont = Instantiate(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
            AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
            audioSrc.Play();
            Destroy(audioCont, 2f);
            //Destroy(gameObject);
        }
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