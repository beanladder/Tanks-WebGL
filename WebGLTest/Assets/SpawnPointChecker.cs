using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointChecker : MonoBehaviour
{
    public static SpawnPointChecker instance;
    public bool isOccupied = false;

    void Awake(){
        instance = this;
    }

    private void OnTriggerStay(Collider others){
        if (others.CompareTag("Tank")){
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider others){
        if (others.CompareTag("Tank")){
            isOccupied = false;
        }
    }

    void Update()
    {
        // Check if there are any Tanks within the trigger collider
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<Collider>().bounds.size.x / 2);
        bool tankFound = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Tank"))
            {
                tankFound = true;
                break;
            }
        }

        if (!tankFound)
        {
            isOccupied = false;
        }
    }
}
