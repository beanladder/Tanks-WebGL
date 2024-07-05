using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkSquareMovement : MonoBehaviour
{
    public static NetworkSquareMovement Instance;
    [Range(2f, 8f)] public float acceleration;
    public float rotationSpeed = 100f;
    public AudioSource engineSound;
    public float recoilForce = 1000f; // Reference to the AudioSource component for the tank engine sound
    public float maxSpeed = 10f; // Maximum forward/backward speed

    private PhotonView view;
    private Rigidbody rb; // Reference to the Rigidbody component
    private bool isGrounded; // Track whether the tank is grounded

    public Collider leftTrack;
    public Collider rightTrack;

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void Awake()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component

        // Ensure the Rigidbody settings are appropriate
        rb.isKinematic = false;
        rb.drag = 1f;
        rb.angularDrag = 0.5f; // Lowered angular drag
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Set collision detection mode to Continuous
    }

    void FixedUpdate()
    {
        if (view.IsMine)
        {
            // Check if the tank is grounded
            isGrounded = CheckIfGrounded();

            if (isGrounded)
            {
                // Movement based on W and S keys
                float verticalInput = Input.GetAxis("Vertical");

                Debug.Log("Vertical Input: " + verticalInput);

                // Calculate force for forward/backward movement
                Vector3 moveForce = transform.forward * verticalInput * acceleration * 10f; // Adjusted force multiplier

                // If moving backward, reduce the force
                if (verticalInput < 0f)
                {
                    moveForce *= 0.75f; // Reduce speed by a quarter
                }

                // Limit the speed
                if (rb.velocity.magnitude < maxSpeed)
                {
                    rb.AddForce(moveForce, ForceMode.Acceleration); // Use Acceleration force mode for continuous movement
                }
                Debug.Log("Move Force: " + moveForce);

                // Rotation based on A and D keys
                float horizontalInput = Input.GetAxis("Horizontal");

                if (verticalInput < 0f) // If moving backward, inverse horizontal input
                {
                    horizontalInput *= -1f;
                }

                Debug.Log("Horizontal Input: " + horizontalInput);

                if (horizontalInput != 0f)
                {
                    // Adjust rotation speed based on whether the tank is moving
                    float currentRotationSpeed = rotationSpeed;
                    if (rb.velocity.magnitude > 0.1f)
                    {
                        currentRotationSpeed *= 1f; // Slow down rotation when moving
                    }
                    else
                    {
                        currentRotationSpeed *= 0.5f; // Speed up rotation when stationary
                    }

                    float rotationAmount = horizontalInput * currentRotationSpeed * Time.fixedDeltaTime;
                    transform.Rotate(Vector3.up, rotationAmount);
                    Debug.Log("Rotation Amount: " + rotationAmount);
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
    }

    private bool CheckIfGrounded()
    {
        bool leftTrackGrounded = CheckTrackCollision(leftTrack);
        bool rightTrackGrounded = CheckTrackCollision(rightTrack);
        return leftTrackGrounded || rightTrackGrounded;
    }

    private bool CheckTrackCollision(Collider track)
    {
        Collider[] colliders = Physics.OverlapBox(track.bounds.center, track.bounds.extents, track.transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }
}







