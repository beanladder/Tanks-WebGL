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
            // Get the position of the tank barrel in world space
            Vector3 targetPosition = tankBarrel.transform.position;

            // Convert world space position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

            // Set the crosshair position to the converted screen position
            transform.position = screenPosition;
        }
    }
}
