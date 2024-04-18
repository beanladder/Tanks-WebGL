using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    
    public float minX;
    public float maxX;
    public float fixedY;
    
    void Start(){
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), fixedY);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

    }
}
