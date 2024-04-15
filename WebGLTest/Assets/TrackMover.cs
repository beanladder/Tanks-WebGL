using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{
    public float movementSpeed = 1f; // Speed of the belt movement
    public float trackWidth = 1f; // Width of the belt track
    public float gapBetweenSquares = 0.1f; // Gap between squares

    private Vector3[] targetPositions; // Target positions for each square

    void Start()
    {
        // Initialize target positions array
        targetPositions = new Vector3[transform.childCount];

        // Calculate initial target positions for each square
        for (int i = 0; i < targetPositions.Length; i++)
        {
            targetPositions[i] = transform.GetChild(i).position;
        }
    }

    void Update()
    {
        // Update target positions for each square
        for (int i = 0; i < targetPositions.Length; i++)
        {
            // Move each target position downward by trackWidth + gapBetweenSquares
            targetPositions[i] += -Vector3.forward * (trackWidth + gapBetweenSquares) * Time.deltaTime * movementSpeed;

            // If the square moves below the belt level, reset its position to the top
            if (targetPositions[i].z < transform.position.z - trackWidth)
            {
                targetPositions[i].z = transform.position.z + gapBetweenSquares;
            }
        }

        // Move each square towards its target position
        for (int i = 0; i < targetPositions.Length; i++)
        {
            Transform square = transform.GetChild(i);
            square.position = Vector3.MoveTowards(square.position, targetPositions[i], movementSpeed * Time.deltaTime);
        }
    }
}
