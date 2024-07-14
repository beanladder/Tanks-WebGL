using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject targetObject; // Reference to the target object placed at the raycast hit point
    public float smoothTime = 0.2f; // Smoothing time for the crosshair movement

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Ensure targetObject reference is not null
        if (targetObject != null)
        {
            // Get the position of the target object in world space
            Vector3 targetPosition = targetObject.transform.position;

            // Convert world space position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

            // Smoothly move the crosshair to the screen position
            transform.position = Vector3.SmoothDamp(transform.position, screenPosition, ref velocity, smoothTime);
        }
    }
}
