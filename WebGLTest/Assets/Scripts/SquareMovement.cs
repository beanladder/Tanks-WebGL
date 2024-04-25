using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class SquareMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float accelerationRotationAmount = 1f; // Rotation amount when accelerating
    public float brakingRotationAmount = -1f; // Rotation amount when braking
    public AudioSource engineSound; // Reference to the AudioSource component for the tank engine sound

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

            // Adjust engine sound pitch based on movement
            if (engineSound != null)
            {
                // Calculate the magnitude of movement (speed)
                float movementMagnitude = Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput);
            if (movementMagnitude == 0)
                {
                    movementMagnitude = 0.1f;
                }

                // Adjust pitch based on movement magnitude
                float minPitch = 0.7f; // Minimum pitch value
                float maxPitch = 1.7f; // Maximum pitch value
                float pitch = Mathf.Lerp(minPitch, maxPitch, movementMagnitude);
                engineSound.pitch = pitch;
                
                // Adjust volume based on movement magnitude (optional)
                float minVolume = 0.7f; // Minimum volume value
                float maxVolume = 1f; // Maximum volume value
                float volume = Mathf.Lerp(minVolume, maxVolume, movementMagnitude);
                engineSound.volume = volume;
                
                // Play or stop the engine sound based on movement
                if (movementMagnitude > 0)
                {
                    if (!engineSound.isPlaying)
                    {
                        engineSound.Play();
                        
                        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;
                        transform.Rotate(Vector3.up, rotationAmount);
                    }
                }
                else
                {
                    if (engineSound.isPlaying)
                    {
                        engineSound.Stop();
                    }
                }
            }
    }
}

              
    




