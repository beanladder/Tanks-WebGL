using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public float rotationSpeed = 10f;

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
            direction.y = 0f; // Keep the Y component constant

            // Rotate the turret to face the direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
