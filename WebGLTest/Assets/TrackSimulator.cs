using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSimulator : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints defining the path
    public float speed = 2f; // Speed of movement along the path

    private int currentWaypointIndex = 0; // Index of the current waypoint

    void Update()
    {
        // Check if there are waypoints defined
        if (waypoints.Length == 0)
            return;

        // Calculate the position towards the current waypoint
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the object has reached the current waypoint
        if (transform.position == targetPosition)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
