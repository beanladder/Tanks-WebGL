using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkSquareMovement : MonoBehaviour
{
    public static NetworkSquareMovement Instance;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float accelerationRotationAmount = 1f; // Rotation amount when accelerating
    public float brakingRotationAmount = -1f; // Rotation amount when braking
    public AudioSource engineSound;
    public float recoilForce = 1000f;// Reference to the AudioSource component for the tank engine sound
    PhotonView view;
    private Quaternion initialRotation; // Initial rotation of the tank

    void Start(){
        view = GetComponent<PhotonView>();
    }

    void Awake()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        initialRotation = transform.rotation; // Store the initial rotation
    }

    void Update()
    {
            if(view.IsMine){
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

        

            float movementMagnitude = Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput);

            if (engineSound != null)
            {
                // Adjust pitch based on movement direction and magnitude
                float minPitch = 0.7f; // Minimum pitch value
                float maxPitch = 1.7f; // Maximum pitch value
                float pitch = 0f;

                if (movementMagnitude > 0)
                {
                    // Calculate pitch based on movement direction
                    if (verticalInput > 0) // Moving forward
                    {
                        pitch = Mathf.Lerp(minPitch, maxPitch, movementMagnitude);
                    }
                    else // Moving backward or strafing
                    {
                        pitch = Mathf.Lerp(minPitch, maxPitch, movementMagnitude / 2f);
                    }
                }

                // Set the engine sound pitch
                engineSound.pitch = pitch;

                // Adjust volume based on movement magnitude (optional)
                float minVolume = 0.7f; // Minimum volume value
                float maxVolume = 1f; // Maximum volume value
                float volume = Mathf.Lerp(minVolume, maxVolume, movementMagnitude);
                engineSound.volume = volume;

                // Play or stop the engine sound based on movement
                if (movementMagnitude > 0 && !engineSound.isPlaying)
                {
                    engineSound.Play();
                }
                else if (movementMagnitude == 0 && engineSound.isPlaying)
                {
                    engineSound.Stop();
                }
            }
        }
    }

    public void ShootProjectile()
    {
        // Create and shoot the projectile
        // For demonstration purposes, I'm just applying a recoil force

        // Apply recoil force in the opposite direction of the tank's forward vector
        GetComponent<Rigidbody>().AddForce(-transform.forward * recoilForce, ForceMode.Impulse);
    }
}







