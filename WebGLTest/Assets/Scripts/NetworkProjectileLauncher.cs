using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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

    public TextMeshProUGUI bulletText1;
    public Image reloadRing1;
    public TextMeshProUGUI bulletText2;
    public Image reloadRing2;


    public GameObject reloadSoundObject;

    private AudioSource audioSource;
    private Vector3 originalTurretPosition;
    private bool canFirePrimary = true;
    private bool canFireSecondary = true;

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
        reloadRing1.fillAmount = 0;
        reloadRing2.fillAmount = 0;
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && canFirePrimary)
            {
                int shooterID = PhotonNetwork.LocalPlayer.ActorNumber;
                view.RPC("FireProjectile", RpcTarget.All, shooterID);
                view.RPC("RecoilAnimation", RpcTarget.All);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                cooldownCoroutine = StartCoroutine(Cooldown(bulletText1, reloadRing1, isPrimary: true));
            }
            else if (Input.GetMouseButtonDown(1) && canFireSecondary)
            {
                view.RPC("FireSmokeGrenade", RpcTarget.All);
                view.RPC("RecoilAnimation", RpcTarget.All);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                cooldownCoroutine = StartCoroutine(Cooldown(bulletText2, reloadRing2, isPrimary: false));
            }
        }
    }



    IEnumerator Cooldown(TextMeshProUGUI bulletText, Image reloadRing, bool isPrimary)
    {
        if (reloadSoundObject != null)
        {
            AudioSource reloadAudioSource = reloadSoundObject.GetComponent<AudioSource>();
            if (reloadAudioSource != null)
            {
                reloadAudioSource.Play();
            }
        }

        if (isPrimary)
        {
            canFirePrimary = false;
        }
        else
        {
            canFireSecondary = false;
        }

        bulletText.gameObject.SetActive(false);  // Hide the bullet text
        reloadRing.fillAmount = 0;  // Ensure the ring starts empty
        float elapsedTime = 0f;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            reloadRing.fillAmount = Mathf.Clamp01(elapsedTime / cooldownTime);
            yield return null;
        }

        reloadRing.fillAmount = 0;  // Ensure the ring is empty after cooldown
        bulletText.gameObject.SetActive(true);  // Show the bullet text again

        if (isPrimary)
        {
            canFirePrimary = true;
        }
        else
        {
            canFireSecondary = true;
        }
    }





    private void FireProjectile()

    [PunRPC]
    public void FireProjectile(int shooterID)

    {
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, firePoint.rotation);
        NetworkProjectile networkProjectile = projectile.GetComponent<NetworkProjectile>();
        if (networkProjectile != null)
        {
            networkProjectile.photonView.RPC("SetShooterID", RpcTarget.All, shooterID);
        }

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
