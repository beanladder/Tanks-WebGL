using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;// Reference to the projectile prefab
    public GameObject BarrelSmokePrefab;
    public GameObject BarrelFlashPrefab;
    public GameObject SmokeGrenedePrefab;
    public GameObject trailPrefab; // Reference to the Trail prefab
    public Transform firePoint; // Reference to the point where the projectile will be instantiated
    public GameObject turret; // Reference to the turret GameObject
    public GameObject target; // Reference to the target GameObject
    public float force = 10f; // Force to be applied to the projectile
    public float speed = 5f; // Speed of the projectile
    public float recoilDistance = 0.1f; // Distance for turret recoil
    public float recoilDuration = 0.1f; // Duration of recoil animation
    public float trailDuration = 2f;
    public float cooldownTime = 1f; // Cooldown time in seconds

    private AudioSource audioSource; // Reference to the AudioSource component
    private Vector3 originalTurretPosition; // Original position of the turret
    private bool canFire = true; // Flag to control firing cooldown
    private Coroutine cooldownCoroutine; // Reference to the cooldown coroutine
    public bool isSmoke = false;
    public static ProjectileLauncher instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        originalTurretPosition = turret.transform.localPosition; // Store the original position of the turret

        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canFire) // Change to 0 for left mouse button, 1 for right mouse button
        {
            // Call FireProjectile method when left mouse button is pressed and firing is allowed
            FireProjectile();
            // Trigger recoil animation
            RecoilAnimation();

            // Play the audio
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Start the cooldown coroutine
            cooldownCoroutine = StartCoroutine(Cooldown());
        }
        else if (Input.GetMouseButtonDown(1) && canFire) // Change to 1 for right mouse button
        {
            // Call FireSmokeGrenade method when right mouse button is pressed and firing is allowed
            FireSmokeGrenade();
            // Trigger recoil animation
            RecoilAnimation();

            // Play the audio
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Start the cooldown coroutine
            cooldownCoroutine = StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        // Set the firing flag to false to prevent further firing during cooldown
        canFire = false;

        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Reset the firing flag to allow firing again
        canFire = true;
    }

    public void FireProjectile()
    {
        isSmoke = false;
        // Instantiate projectile at the fire point
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Instantiate trail effect
        GameObject trailEffect = Instantiate(trailPrefab, firePoint.position, firePoint.rotation);

        // Parent the trail effect to the projectile
        trailEffect.transform.parent = projectile.transform;

        // Calculate direction to the target
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        // Apply force to the rigidbody of the projectile
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            // Apply initial force with adjusted magnitude to control speed and distance
            projectileRigidbody.velocity = direction * speed;

            // Apply a damping effect to gradually reduce the velocity
            projectileRigidbody.angularDrag = 1f;
            projectileRigidbody.drag = projectileRigidbody.angularDrag - 0.5f;// Adjust the value as needed
        }

        // Destroy the trail effect after a certain duration
        Destroy(trailEffect, trailDuration);

        GameObject smoke = Instantiate(BarrelSmokePrefab, firePoint.position, firePoint.rotation);
        GameObject flash = Instantiate(BarrelFlashPrefab, firePoint.position, firePoint.rotation);
        Destroy(flash, 2f);
        Destroy(smoke, 2f);
    }

    public void RecoilAnimation()
    {
        // Move the turret back along the Z-axis
        LeanTween.moveLocalZ(turret, originalTurretPosition.z - recoilDistance, recoilDuration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setOnComplete(() =>
                 {
                     // Move the turret back to its original position
                     LeanTween.moveLocalZ(turret, originalTurretPosition.z, recoilDuration)
                              .setEase(LeanTweenType.easeInQuad);
                 });

        //SquareMovement.Instance.ShootProjectile(); 
    }
    public void FireSmokeGrenade()
    {
        isSmoke = true;
        GameObject smokeGrenade = Instantiate(SmokeGrenedePrefab, firePoint.position, firePoint.rotation);

        // Calculate direction to the target
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        // Apply force to the rigidbody of the smoke grenade
        Rigidbody grenadeRigidbody = smokeGrenade.GetComponent<Rigidbody>();
        if (grenadeRigidbody != null)
        {
            // Apply initial force with adjusted magnitude to control speed and distance
            grenadeRigidbody.velocity = direction * speed;

            // Apply a damping effect to gradually reduce the velocity
            grenadeRigidbody.angularDrag = 1f;
            grenadeRigidbody.drag = grenadeRigidbody.angularDrag - 0.5f; // Adjust the value as needed
        }

        
    }
}

