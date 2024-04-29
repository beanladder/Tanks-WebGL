using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;


public class OldNetworkProjectile : MonoBehaviour
{
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public AudioSource TankHit;
    public GameObject boomPrefab; // Prefab to instantiate when hitting a tank
    public int DamageAmt = 5;
    PhotonView view;
    void Start(){
        view = GetComponent<PhotonView>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            StartCoroutine(PlayAudio("Boom"));
        }
        else if (collision.gameObject.CompareTag("Cylinder"))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            StartCoroutine(PlayAudio("Ricochet"));
        }
        else if (collision.gameObject.CompareTag("Tank"))
        {
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = Instantiate(boomPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(explosion, 2f);
            //collision.gameObject.GetComponent<TankInfo>().TakeDamage(DamageAmt);
            collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage",RpcTarget.All,DamageAmt);
            StartCoroutine(InitiateDamage());
        }
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(1f);
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
        TankHit.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}