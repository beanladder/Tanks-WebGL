using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;


public class NetworkProjectileLauncher : MonoBehaviourPunCallbacks
{
    public GameObject projectilePrefab;
    public GameObject barrelSmokePrefab;
    public GameObject barrelFlashPrefab;
    public GameObject smokeGrenadePrefab;
    public Transform firePoint;
    public GameObject turret;
    public GameObject target;
    public float speed = 5f;
    public float recoilDistance = 0.1f;
    public float recoilDuration = 0.1f;
    public float cooldownTime = 1f;

    public AudioClip fireSound;
    public AudioClip reloadSound;

    private Vector3 originalTurretPosition;
    private bool canFire = true;
    private PhotonView view;
    private AudioSource audioSource;

    private ObjectPool projectilePool;
    private ObjectPool barrelSmokePool;
    private ObjectPool barrelFlashPool;
    private ObjectPool smokeGrenadePool;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
        originalTurretPosition = turret.transform.localPosition;

        // Initialize object pools
        projectilePool = new ObjectPool(projectilePrefab, 10);
        barrelSmokePool = new ObjectPool(barrelSmokePrefab, 5);
        barrelFlashPool = new ObjectPool(barrelFlashPrefab, 5);
        smokeGrenadePool = new ObjectPool(smokeGrenadePrefab, 5);
    }

    void Update()
    {
        if (!view.IsMine) return;

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            photonView.RPC("FireProjectile", RpcTarget.All);
        }
        else if (Input.GetMouseButtonDown(1) && canFire)
        {
            FireSmokeGrenade();
        }
    }

    [PunRPC]
    void FireProjectile()
    {
        if (!canFire) return;

        GameObject projectile = projectilePool.GetObject();
        if (projectile != null)
        {
            projectile.SetActive(true);
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = firePoint.rotation;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb == null) rb = projectile.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 direction = (target.transform.position - firePoint.position).normalized;
            rb.AddForce(direction * speed, ForceMode.VelocityChange);

            NetworkProjectile networkProjectile = projectile.GetComponent<NetworkProjectile>();
            if (networkProjectile != null)
            {
                networkProjectile.InitializeProjectile(view.ViewID);
            }

            if (PhotonNetwork.IsMessageQueueRunning)
            {
                photonView.RPC("SyncProjectile", RpcTarget.Others, projectile.GetComponent<PhotonView>().ViewID, firePoint.position, direction);
            }
        }

        SpawnEffect(barrelSmokePool);
        SpawnEffect(barrelFlashPool);
        PlaySound(fireSound);
        RecoilAnimation();
        StartCoroutine(Cooldown());
    }

    [PunRPC]
    void SyncProjectile(int viewID, Vector3 position, Vector3 direction)
    {
        PhotonView projectileView = PhotonView.Find(viewID);
        if (projectileView != null)
        {
            GameObject projectile = projectileView.gameObject;
            projectile.transform.position = position;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }
    }

    [PunRPC]
    void FireSmokeGrenade()
    {
        if (!canFire) return;

        GameObject smokeGrenade = smokeGrenadePool.GetObject();
        smokeGrenade.transform.position = firePoint.position;
        smokeGrenade.transform.rotation = firePoint.rotation;

        Vector3 direction = (target.transform.position - firePoint.position).normalized;
        Rigidbody grenadeRb = smokeGrenade.GetComponent<Rigidbody>();
        if (grenadeRb != null)
        {
            grenadeRb.velocity = direction * speed;
        }

        PlaySound(fireSound);
        RecoilAnimation();
        StartCoroutine(Cooldown());
    }

    void SpawnEffect(ObjectPool pool)
    {
        GameObject effect = pool.GetObject();
        effect.transform.position = firePoint.position;
        effect.transform.rotation = firePoint.rotation;
        StartCoroutine(ReturnToPool(effect, pool, 2f));
    }

    IEnumerator ReturnToPool(GameObject obj, ObjectPool pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.ReturnObject(obj);
    }

    void RecoilAnimation()
    {
        LeanTween.moveLocalZ(turret, originalTurretPosition.z - recoilDistance, recoilDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.moveLocalZ(turret, originalTurretPosition.z, recoilDuration)
                    .setEase(LeanTweenType.easeInQuad);
            });
    }

    IEnumerator Cooldown()
    {
        canFire = false;
        PlaySound(reloadSound);
        yield return new WaitForSeconds(cooldownTime);
        canFire = true;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

// Helper class for object pooling
public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Object.Instantiate(prefab);
            return obj;
        }

        GameObject pooledObject = pool.Dequeue();
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
