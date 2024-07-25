using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public Transform barrelEnd; // Assign the barrel end in the Unity editor
    public GameObject targetDirectionObject; // Assign the target GameObject that defines the direction in the Unity editor
    public GameObject targetrrrr; // Assign the object to be placed at hit points
    public LayerMask layerMask; // Specify the layer(s) you want to check for
    public float maxDistance = 100f; // Maximum distance for the raycast

    void Update()
    {
        // Check if the necessary objects are assigned
        if (barrelEnd == null || targetDirectionObject == null || targetrrrr == null)
        {
            Debug.LogError("Barrel end, target direction object, or targetrrrr object is not assigned!");
            return;
        }

        // Calculate the direction from barrel end to the target direction object
        Vector3 rayDirection = (targetDirectionObject.transform.position - barrelEnd.position).normalized;

        // Shoot a raycast from barrel end position in the calculated direction
        RaycastHit hit;
        if (Physics.Raycast(barrelEnd.position, rayDirection, out hit, maxDistance, layerMask))
        {
            // If the raycast hits an object within maxDistance
            // Place the targetrrrr object at the hit point
            targetrrrr.transform.position = hit.point;
            //Debug.Log($"Hit object: {hit.collider.gameObject.name}, Hit point: {hit.point}");
        }
        else
        {
            // If the raycast doesn't hit anything, place the targetrrrr object at the maximum distance
            targetrrrr.transform.position = barrelEnd.position + rayDirection * maxDistance;
        }

        // Visualize the ray
        Debug.DrawRay(barrelEnd.position, rayDirection * maxDistance, Color.red);
    }
}