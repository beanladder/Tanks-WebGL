using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointChecker : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isOccupied = false;

    private void OnTriggerEnter(Collider others){
        if(others.CompareTag("Tank")){
            isOccupied = true;
        }
    }
    private void OnTriggerExit(Collider others){
        if(others.CompareTag("Tank")){
            isOccupied = false;
        }
    }
}
