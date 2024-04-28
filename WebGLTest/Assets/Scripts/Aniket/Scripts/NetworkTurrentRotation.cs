using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NetworkTurretRotation : MonoBehaviour
{
    public float turnSpeed = 15f;
    public AudioSource audioSource; // Reference to the AudioSource component

    Camera maincam;
    bool isRotating = false; // Flag to track if the turret is currently rotating
    float rotationThreshold = 1f; // Threshold angle to determine when the turret stops rotating

    public float xRotation,zRotation;
    public float yRotation;
    public PhotonView view;

    void Start()
    {
        maincam = Camera.main;
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine){
                float yawCamera = maincam.transform.rotation.eulerAngles.y;

            // Target rotation with fixed X (180) and Z (0)
            Quaternion targetRotation = Quaternion.Euler(xRotation, yawCamera+yRotation, zRotation);

            // Check if the turret is rotating
            if (Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold)
            {
                isRotating = true;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            else
            {
                isRotating = false;
            }

            // Play or stop the audio based on whether the turret is rotating
            if (audioSource != null)
            {
                if (isRotating && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                else if (!isRotating && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}
