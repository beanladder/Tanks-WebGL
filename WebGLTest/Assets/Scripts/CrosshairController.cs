using UnityEngine;
using UnityEngine.UI;
public class CrosshairController : MonoBehaviour
{
    public GameObject tankBarrel; // Reference to the tank barrel GameObject

    void Update()
    {
        // Ensure tankBarrel reference is not null
        if (tankBarrel != null)
        {
            // Get the position of the tank barrel
            Vector3 targetPosition = tankBarrel.transform.position;

            // Set the crosshair position to match the tank barrel position
            transform.position = targetPosition;

            // Make the crosshair always face the camera
            transform.LookAt(Camera.main.transform);
        }
    }
}