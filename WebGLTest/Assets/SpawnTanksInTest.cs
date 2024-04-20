using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTanksInTest : MonoBehaviour
{
    public GameObject tankPrefab; // Reference to the tank prefab to spawn
    public int numberOfTanks = 2; // Number of tanks to spawn
    public float spawnRadius = 10f; // Radius around the center to spawn tanks

    void Start()
    {
        // Spawn tanks
        SpawnTanks();
    }

    void SpawnTanks()
    {
        // Get the center of the map
        Vector3 center = transform.position;

        // Iterate to spawn each tank
        for (int i = 0; i < numberOfTanks; i++)
        {
            // Calculate random position within spawn radius
            Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = center + new Vector3(randomOffset.x, 0f, randomOffset.y);

            // Instantiate tank prefab at the random position
            Instantiate(tankPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
