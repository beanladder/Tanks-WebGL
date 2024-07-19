// using System.Collections;
// using System.Collections.Generic;
// using Photon.Pun;
// using UnityEngine;

// public class NetworkTurretRotation : MonoBehaviour
// {
//     public float turnSpeed = 15f;
//     public AudioSource audioSource; // Reference to the AudioSource component

//     Camera maincam;
//     bool isRotating = false; // Flag to track if the turret is currently rotating
//     float rotationThreshold = 1f; // Threshold angle to determine when the turret stops rotating

//     public float xRotation, zRotation;
//     public float yRotation;
//     PhotonView view;

//     void Start()
//     {
//         maincam = Camera.main;
//         view = GetComponent<PhotonView>();
//     }

//     void Update()
//     {
//         if (view.IsMine)
//         {
//             // Calculate the camera's yaw rotation
//             float yawCamera = maincam.transform.rotation.eulerAngles.y;

//             // Calculate the target rotation based on camera yaw
//             Quaternion targetRotation = Quaternion.Euler(xRotation, yawCamera + yRotation, zRotation);

//             // Check if the turret is rotating
//             if (Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold)
//             {
//                 isRotating = true;
//                 // Rotate turret towards target rotation
//                 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
//             }
//             else
//             {
//                 isRotating = false;
//             }

//             // Update audio source based on rotation status
//             if (audioSource != null)
//             {
//                 if (isRotating && !audioSource.isPlaying)
//                 {
//                     audioSource.Play();
//                 }
//                 else if (!isRotating && audioSource.isPlaying)
//                 {
//                     audioSource.Stop();
//                 }
//             }

//             // Ensure turret's rotation does not get affected by the rotation of the parent objects
//             // Reset turret's local rotation relative to its parent (TurretPivot)
//             Transform turretPivot = transform.parent;
//             if (turretPivot != null)
//             {
//                 transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
//             }
//         }
//     }
// }
using Photon.Pun;
using UnityEngine;

public class NetworkTurretRotation : MonoBehaviourPunCallbacks, IPunObservable
{
    public float turnSpeed = 15f;
    public AudioSource audioSource;

    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float rotationThreshold = 1f;

    private Camera mainCam;
    private bool isRotating = false;
    private Quaternion networkRotation;
    private float xRotation, zRotation;
    private float yRotation;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            LocalRotation();
        }
        else
        {
            RemoteRotation();
        }

        UpdateAudio();
    }

    private void LocalRotation()
    {
        float yawCamera = mainCam.transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(xRotation, yawCamera + yRotation, zRotation);

        isRotating = Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold;

        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        AdjustLocalRotation();
    }

    private void RemoteRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, smoothing * Time.deltaTime);
        isRotating = Quaternion.Angle(transform.rotation, networkRotation) > rotationThreshold;
    }

    private void UpdateAudio()
    {
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

    private void AdjustLocalRotation()
    {
        Transform turretPivot = transform.parent;
        if (turretPivot != null)
        {
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
