using Photon.Pun;
using UnityEngine;

public class NetworkRotateObject : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float minRotation = 60.0f;
    public float maxRotation = 100.0f;
    PhotonView view;

    void Start(){
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine){
                // Get the mouseY movement
            float mouseY = -Input.GetAxis("Mouse Y");

            // Calculate the rotation amount based on mouseY movement
            float rotationAmount = mouseY * rotationSpeed * Time.deltaTime;

            // Apply rotation along the x-axis
            transform.Rotate(Vector3.right, rotationAmount);

            // Clamp the rotation between minRotation and maxRotation
            Vector3 currentRotation = transform.localRotation.eulerAngles;
            currentRotation.x = Mathf.Clamp(currentRotation.x, minRotation, maxRotation);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }
}
