using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject tankBarrel; // Reference to the tank barrel GameObject
    public float smoothSpeed = 0.125f; // Smoothing speed with a range slider in the Inspector
    public float minThreshold = 5.0f; // Minimum distance threshold for moving the crosshair
    private Vector3 previousScreenPosition;

    void Start()
    {
        previousScreenPosition = transform.position;
    }

    void LateUpdate()
    {
        // Ensure tankBarrel reference is not null
        if (tankBarrel != null)
        {
            // Get the position of the tank barrel in world space
            Vector3 targetPosition = tankBarrel.transform.position;

            // Convert world space position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

            // Calculate the distance from the previous screen position
            float distance = Vector3.Distance(previousScreenPosition, screenPosition);

            // Move the crosshair if the distance exceeds the minimum threshold
            if (distance > minThreshold)
            {
                transform.position = Vector3.Lerp(transform.position, screenPosition, smoothSpeed);
                previousScreenPosition = screenPosition;
            }
        }
    }
}
