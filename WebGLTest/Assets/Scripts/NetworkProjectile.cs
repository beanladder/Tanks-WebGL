using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
public class NetworkProjectile : MonoBehaviourPunCallbacks
{
    public AudioSource Boom;
    public AudioSource[] Ricochet;
    public GameObject TankHitAudio;
    public GameObject boomPrefab;
    public GameObject hitwallPrefab;

    public event Action OnHitTank;
    private int shooterID;
    private int damageAmt;

    [PunRPC]
    public void SetShooterID(int id)
    {
        shooterID = id;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleProjectileCollision(collision);
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
            OnHitTank?.Invoke();
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = ObjectPool.Instance.GetObject(boomPrefab, collision.contacts[0].point, Quaternion.identity);
            ObjectPool.Instance.ReleaseObject(explosion, 2f);
            GameObject audioCont = ObjectPool.Instance.GetObject(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
            AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
            audioSrc.Play();
            ObjectPool.Instance.ReleaseObject(audioCont, 2f);
            damageAmt = UnityEngine.Random.Range(4, 9);
            Vector3 impactPosition = collision.contacts[0].point;
            float impulseForce = damageAmt / 5f;
            Vector3 impulseDirection = (impactPosition - transform.position).normalized;

            PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
            if (targetView != null)
            {
                Player shooter = PhotonNetwork.CurrentRoom.GetPlayer(shooterID);
                string shooterName = shooter != null ? shooter.NickName : "Unknown";
                string hitTankName = targetView.Owner.NickName;

                Debug.LogWarning($"Projectile owned by {shooterName} hit tank owned by {hitTankName}");

                targetView.RPC("TakeDamage", RpcTarget.All, damageAmt);
                targetView.RPC("ShakeCamera", RpcTarget.All, impactPosition, impulseDirection, impulseForce);
            }
            ObjectPool.Instance.ReleaseObject(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            GameObject explosion = ObjectPool.Instance.GetObject(hitwallPrefab, collision.contacts[0].point, Quaternion.identity);
            ObjectPool.Instance.ReleaseObject(explosion, 2f);
            GameObject audioCont = ObjectPool.Instance.GetObject(TankHitAudio, collision.contacts[0].point, Quaternion.identity);
            AudioSource audioSrc = audioCont.GetComponent<AudioSource>();
            audioSrc.Play();
            ObjectPool.Instance.ReleaseObject(audioCont, 2f);
            ObjectPool.Instance.ReleaseObject(gameObject);
        }
    }

    public IEnumerator PlayAudio(string ID)
    {
        if (ID == "Boom")
        {
            Boom.Play();
            yield return new WaitForSeconds(0.5f);
            ObjectPool.Instance.ReleaseObject(gameObject);
        }
        else if (ID == "Ricochet")
        {
            int chance = UnityEngine.Random.Range(0, Ricochet.Length);
            Ricochet[chance].Play();
        }
    }
}

