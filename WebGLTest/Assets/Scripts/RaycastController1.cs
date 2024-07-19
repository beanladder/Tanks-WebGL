using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public Transform barrelEnd; // Assign the barrel end in the Unity editor
    public GameObject targetObject; // Assign the target GameObject in the Unity editor
    public LayerMask layerMask; // Specify the layer(s) you want to check for

    public float maxDistance = 100f; // Maximum distance for the raycast

    void Update()
    {
        // Check if the barrel end and target object are assigned
        if (barrelEnd == null || targetObject == null)
        {
            Debug.LogError("Barrel end or target object is not assigned!");
            return;
        }

        // Calculate the direction from barrel end to the target object
        Vector3 rayDirection = targetObject.transform.position - barrelEnd.position;

        // Shoot a raycast from barrel end position towards the target object
        RaycastHit hit;
        if (Physics.Raycast(barrelEnd.position, rayDirection, out hit, maxDistance, layerMask))
        {
            // If the raycast hits an object within maxDistance
            // Place the target object at the hit point
            targetObject.transform.position = hit.point;
            Debug.Log($"Hit object: {hit.collider.gameObject.name}, Transform: {hit.collider.transform.position}");

        }
        else
        {
            // If the raycast doesn't hit anything, place the target object at the maximum distance
            targetObject.transform.position = barrelEnd.position + rayDirection.normalized * maxDistance;
        }

        // Visualize the ray
        Debug.DrawRay(barrelEnd.position, rayDirection.normalized * maxDistance, Color.red);
    }
}
