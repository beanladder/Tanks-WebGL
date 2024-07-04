using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkProjectileLauncher : MonoBehaviourPunCallbacks
{
    public GameObject projectilePrefab;
    public GameObject BarrelSmokePrefab;
    public GameObject BarrelFlashPrefab;
    public GameObject SmokeGrenadePrefab;
    public GameObject trailPrefab;
    public Transform firePoint;
    public GameObject turret;
    public GameObject target;
    public float force = 10f;
    public float speed = 5f;
    public float recoilDistance = 0.1f;
    public float recoilDuration = 0.1f;
    public float trailDuration = 2f;
    public float cooldownTime = 1f;

    private AudioSource audioSource;
    private Vector3 originalTurretPosition;
    private bool canFire = true;
    private Coroutine cooldownCoroutine;
    PhotonView view;
    public static NetworkProjectileLauncher instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalTurretPosition = turret.transform.localPosition;
        view = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && canFire)
            {
                view.RPC("FireProjectile", RpcTarget.All);
                view.RPC("RecoilAnimation", RpcTarget.All);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                cooldownCoroutine = StartCoroutine(Cooldown());
            }
            else if (Input.GetMouseButtonDown(1) && canFire)
            {
                view.RPC("FireSmokeGrenade", RpcTarget.All);
                view.RPC("RecoilAnimation", RpcTarget.All);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                cooldownCoroutine = StartCoroutine(Cooldown());
            }
        }
    }

    IEnumerator Cooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(cooldownTime);
        canFire = true;
    }

    [PunRPC]
    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        GameObject trailEffect = Instantiate(trailPrefab, firePoint.position, firePoint.rotation);
        trailEffect.transform.parent = projectile.transform;

        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.velocity = direction * speed;
            projectileRigidbody.angularDrag = 1f;
            projectileRigidbody.drag = projectileRigidbody.angularDrag - 0.5f;
        }

        Destroy(trailEffect, trailDuration);
        GameObject smoke = Instantiate(BarrelSmokePrefab, firePoint.position, firePoint.rotation);
        GameObject flash = Instantiate(BarrelFlashPrefab, firePoint.position, firePoint.rotation);
        Destroy(flash, 2f);
        Destroy(smoke, 2f);
    }

    [PunRPC]
    public void RecoilAnimation()
    {
        LeanTween.moveLocalZ(turret, originalTurretPosition.z - recoilDistance, recoilDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.moveLocalZ(turret, originalTurretPosition.z, recoilDuration)
                    .setEase(LeanTweenType.easeInQuad);
            });
    }

    [PunRPC]
    public void FireSmokeGrenade()
    {
        GameObject smokeGrenade = Instantiate(SmokeGrenadePrefab, firePoint.position, firePoint.rotation);
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        Rigidbody grenadeRigidbody = smokeGrenade.GetComponent<Rigidbody>();
        if (grenadeRigidbody != null)
        {
            grenadeRigidbody.velocity = direction * speed;
            grenadeRigidbody.angularDrag = 1f;
            grenadeRigidbody.drag = grenadeRigidbody.angularDrag - 0.5f;
        }
    }
}
