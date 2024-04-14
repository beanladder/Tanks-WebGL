using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float barrelMinAngle = 80f;
    public float barrelMaxAngle = 100f;
    private ProjectileLauncher projectileLauncher;

    private void Start()
    {
        projectileLauncher = FindObjectOfType<ProjectileLauncher>();
    }

    void Update()
    {
        // Create a ray from the camera to the mouse cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something in the scene
        if (Physics.Raycast(ray, out hit))
        {
            // Calculate the direction from the turret's position to the hit point
            Vector3 direction = hit.point - transform.position;

            // Ignore rotation around the Y-axis
            direction.y = 0f;

            // Rotate the turret pivot to face the direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Rotate the barrel based on vertical mouse movement
        float mouseY = Input.GetAxis("Mouse Y");
        float barrelRotationAmount = Mathf.Clamp(mouseY, -1f, 1f) * rotationSpeed * Time.deltaTime;
        Transform barrel = transform.Find("Barrel"); // Assuming the barrel is a child of the turret
        if (barrel != null)
        {
            Vector3 newEulerAngles = barrel.localEulerAngles;
            newEulerAngles.x = Mathf.Clamp(newEulerAngles.x + barrelRotationAmount, barrelMinAngle, barrelMaxAngle);
            barrel.localEulerAngles = newEulerAngles;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
        }
    }

    void FireProjectile() {
        // Call the FireProjectile method of the ProjectileLauncher script
        if (projectileLauncher != null)
        {
            projectileLauncher.FireProjectile();
        }
    }
}
