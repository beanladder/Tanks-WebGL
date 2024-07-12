using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool isOccupied;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Spawn point {gameObject.name} is now occupied.");
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Spawn point {gameObject.name} is now unoccupied.");
            isOccupied = false;
        }
    }
}
