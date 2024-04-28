using Photon.Pun;
using UnityEngine;

public class NetworkBarrelUpDown : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float minRotation = -25f;
    public float maxRotation = 50f;
    private float currentRotationX = 0f; // Store the current rotation
    public float MouseY;
    public PhotonView view;

    void Start(){
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine){
            float mouseY = Input.GetAxis("Mouse Y") * MouseY;

        // Rotate around X-axis based on mouse input
        transform.Rotate(Vector3.right, mouseY * rotationSpeed * Time.deltaTime);

        // Get the current rotation around X-axis
        currentRotationX = transform.localEulerAngles.x;

        // Convert rotation to a range of -180 to 180
        if (currentRotationX > 180f)
        {
            currentRotationX -= 360f;
        }

        // Adjust rotation smoothly between -25 and 50 based on mouse input
        if (mouseY > 0) // Moving mouse up
        {
            currentRotationX = Mathf.MoveTowards(currentRotationX, maxRotation, Mathf.Abs(mouseY) * rotationSpeed * Time.deltaTime);
        }
        else if (mouseY < 0) // Moving mouse down
        {
            currentRotationX = Mathf.MoveTowards(currentRotationX, minRotation, Mathf.Abs(mouseY) * rotationSpeed * Time.deltaTime);
        }

        // Apply the adjusted rotation
        transform.localEulerAngles = new Vector3(currentRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
