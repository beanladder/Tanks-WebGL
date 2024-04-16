using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelUpDown : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float minRotation = 60.0f;
    public float maxRotation = 100.0f;

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        float newRotation = transform.rotation.eulerAngles.x - mouseY * rotationSpeed;

        // Ensure rotation is within the desired range
        if (newRotation > 180)
        {
            newRotation -= 360;
        }
        newRotation = Mathf.Clamp(newRotation, minRotation, maxRotation);

        // Apply the new rotation
        transform.rotation = Quaternion.Euler(newRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}