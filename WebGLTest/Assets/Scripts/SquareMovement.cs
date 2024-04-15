using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SquareMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float accelerationRotationAmount = 1f; // Rotation amount when accelerating
    public float brakingRotationAmount = -1f; // Rotation amount when braking

    private Quaternion initialRotation; // Initial rotation of the tank

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialRotation = transform.rotation; // Store the initial rotation
    }

    void Update()
    {
        // Movement based on W and S keys
        float verticalInput = Input.GetAxis("Vertical");
        float moveAmount = verticalInput * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveAmount);

        // Rotation based on A and D keys
        float horizontalInput = Input.GetAxis("Horizontal");
        if (verticalInput < 0f) // If moving backward
        {
            horizontalInput *= -1f; // Reverse horizontal input
        }
        if (horizontalInput != 0f)
        {
            float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }

        // Rotate backward for acceleration (W key)
        if (verticalInput > 0f)
        {
            // Rotate relative to the initial rotation
            LeanTween.rotateX(gameObject, initialRotation.eulerAngles.x + accelerationRotationAmount, 0.1f);
        }
        // Rotate forward for braking (S key)
        else if (verticalInput < 0f)
        {
            // Rotate relative to the initial rotation
            LeanTween.rotateX(gameObject, initialRotation.eulerAngles.x + brakingRotationAmount, 0.1f);
        }
        // Rotate back to the initial rotation if not accelerating or braking
        else
        {
            LeanTween.rotate(gameObject, initialRotation.eulerAngles, 0.1f);
        }
    }
}