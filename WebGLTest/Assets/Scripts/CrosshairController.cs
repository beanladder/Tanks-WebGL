using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public GameObject tankBarrel; // Reference to the tank barrel GameObject

    
    public float smoothSpeed = 0.125f; // Smoothing speed with a range slider in the Inspector

    void Update()
    {
        // Ensure tankBarrel reference is not null
        if (tankBarrel != null)
        {
            // Get the position of the tank barrel in world space
            Vector3 targetPosition = tankBarrel.transform.position;

            // Convert world space position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

            // Smoothly interpolate the crosshair position towards the target screen position
            transform.position = Vector3.Lerp(transform.position, screenPosition, smoothSpeed);
        }
    }
}
