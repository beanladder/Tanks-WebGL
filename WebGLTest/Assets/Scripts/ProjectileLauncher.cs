using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public Transform firePoint; // Reference to the point where the projectile will be instantiated
    public GameObject turret; // Reference to the turret GameObject
    public GameObject target; // Reference to the target GameObject
    public float force = 10f; // Force to be applied to the projectile
    public float speed = 5f; // Speed of the projectile
    public float recoilDistance = 0.1f; // Distance for turret recoil
    public float recoilDuration = 0.1f; // Duration of recoil animation

    private Vector3 originalTurretPosition; // Original position of the turret

    void Start()
    {
        originalTurretPosition = turret.transform.localPosition; // Store the original position of the turret
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Change to 0 for left mouse button, 1 for right mouse button
        {
            // Call FireProjectile method when left mouse button is pressed
            FireProjectile();
            // Trigger recoil animation
            RecoilAnimation();
        }
    }

    public void FireProjectile()
    {
        // Instantiate projectile at the fire point
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Calculate direction to the target
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        // Apply force to the rigidbody of the projectile
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            // Apply force with adjusted magnitude to control speed and distance
            projectileRigidbody.velocity = direction * speed;
        }
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
    }
}
