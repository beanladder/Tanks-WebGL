using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SquareMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

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
    }
}