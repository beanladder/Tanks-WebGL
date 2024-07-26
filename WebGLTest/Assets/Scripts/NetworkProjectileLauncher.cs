using System.Collections;
using Photon.Pun;
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
    public GameObject hitMarker;

    private AudioSource audioSource;
    private Vector3 originalTurretPosition;
    private bool canFirePrimary = true;
    private bool canFireSecondary = true;

    private float hitmarkerDuration = 1.5f;

    private Coroutine cooldownCoroutine;
    private PhotonView view;
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
        hitMarker.SetActive(false);
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && canFirePrimary)
            {
                int shooterID = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 direction = (target.transform.position - firePoint.position).normalized;
                Vector3 velocity = direction * speed;
                
                FireProjectile(shooterID, firePoint.position, firePoint.rotation, velocity);
                
                view.RPC("SyncProjectile", RpcTarget.Others, shooterID, firePoint.position, firePoint.rotation, velocity);
                
                view.RPC("RecoilAnimation", RpcTarget.All);
                PlayAudio();

                cooldownCoroutine = StartCoroutine(Cooldown(bulletText1, reloadRing1, true));
            }
            else if (Input.GetMouseButtonDown(1) && canFireSecondary)
            {
                view.RPC("FireSmokeGrenade", RpcTarget.All);
                view.RPC("RecoilAnimation", RpcTarget.All);
                PlayAudio();
                cooldownCoroutine = StartCoroutine(Cooldown(bulletText2, reloadRing2, false));
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

        bulletText.gameObject.SetActive(false);  
        reloadRing.fillAmount = 0;
        float elapsedTime = 0f;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            reloadRing.fillAmount = Mathf.Clamp01(elapsedTime / cooldownTime);
            yield return null;
        }

        reloadRing.fillAmount = 0;
        bulletText.gameObject.SetActive(true);

        if (isPrimary)
        {
            canFirePrimary = true;
        }
        else
        {
            canFireSecondary = true;
        }
    }

    [PunRPC]
    public void FireProjectile(int shooterID, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        GameObject projectile = ObjectPool.Instance.GetObject(projectilePrefab, position, rotation);
        NetworkProjectile networkProjectile = projectile.GetComponent<NetworkProjectile>();
        if (networkProjectile != null)
        {
            networkProjectile.SetShooterID(shooterID);
            networkProjectile.OnHitTank += ShowHitMarker;
        }

        GameObject trailEffect = ObjectPool.Instance.GetObject(trailPrefab, position, rotation);
        trailEffect.transform.parent = projectile.transform;

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.velocity = velocity;
            projectileRigidbody.angularDrag = 1f;
            projectileRigidbody.drag = projectileRigidbody.angularDrag - 0.5f;
        }

        ObjectPool.Instance.ReleaseObject(trailEffect, trailDuration);
        GameObject smoke = ObjectPool.Instance.GetObject(BarrelSmokePrefab, position, rotation);
        GameObject flash = ObjectPool.Instance.GetObject(BarrelFlashPrefab, position, rotation);
        ObjectPool.Instance.ReleaseObject(flash, 2f);
        ObjectPool.Instance.ReleaseObject(smoke, 2f);
    }

    [PunRPC]
    public void SyncProjectile(int shooterID, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        FireProjectile(shooterID, position, rotation, velocity);
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
        GameObject smokeGrenade = ObjectPool.Instance.GetObject(SmokeGrenadePrefab, firePoint.position, firePoint.rotation);
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        Rigidbody grenadeRigidbody = smokeGrenade.GetComponent<Rigidbody>();
        if (grenadeRigidbody != null)
        {
            grenadeRigidbody.velocity = direction * speed;
            grenadeRigidbody.angularDrag = 1f;
            grenadeRigidbody.drag = grenadeRigidbody.angularDrag - 0.5f;
        }
    }

    private void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void ShowHitMarker()
    {
        if (view.IsMine)
        {
            StartCoroutine(HitmarkerCoroutine());
        }
    }

    private IEnumerator HitmarkerCoroutine()
    {
        hitMarker.SetActive(true);
        yield return new WaitForSeconds(hitmarkerDuration);
        hitMarker.SetActive(false);
    }
}

