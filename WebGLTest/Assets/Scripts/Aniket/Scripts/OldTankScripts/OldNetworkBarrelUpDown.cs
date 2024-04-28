using Photon.Pun;
using UnityEngine;

public class OldNetworkBarrelUpDown : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float minRotation = 60f;
    public float maxRotation = 100f;
    PhotonView view;
    void Start(){
        view = GetComponent<PhotonView>();
    }
    void Update()
    {
        if(view.IsMine){
                float mouseY = -Input.GetAxis("Mouse Y");

            // Rotate around X-axis based on mouse input
            transform.Rotate(Vector3.right, mouseY * rotationSpeed * Time.deltaTime);

            // Get the current rotation around X-axis
            float currentRotationX = transform.localEulerAngles.x;

            // Adjust rotation clamping based on mouse direction
            if (mouseY > 0) // Moving mouse up
            {
                currentRotationX = Mathf.Clamp(currentRotationX, minRotation, maxRotation);
            }
            else if (mouseY < 0) // Moving mouse down
            {
                // Allow rotation down to minRotation but clamp it to maxRotation when it exceeds
                currentRotationX = Mathf.Clamp(currentRotationX, minRotation, 360f - (360f - maxRotation));
            }

            // Apply the adjusted rotation
            transform.localEulerAngles = new Vector3(currentRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
