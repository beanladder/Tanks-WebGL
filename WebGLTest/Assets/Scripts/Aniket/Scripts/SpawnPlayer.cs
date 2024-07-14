using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public static SpawnPlayer instance;
    public GameObject playerPrefab;
    private GameObject player;
    public GameObject SpawnScreen;
    public GameObject RespawnScreen;
    public Transform[] spawnPoints;
    public float sphereRadius=10f;
    private int deadTankId = -1;
    private bool isTankDead = false;
    private PhotonView view;

     void Awake()
     {
         instance = this;
     }

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in SpawnPlayer script!");
            return;
        }
        // CameraManager.instance.BlendToMapCamera();
    }
    void Update()
    {
        if(isTankDead && PhotonNetwork.LocalPlayer.ActorNumber == deadTankId){
            RespawnScreen.SetActive(true);
        }
        else{
            RespawnScreen.SetActive(false);
        }
    }

    public void SetDeadTankId(int tankID){
        deadTankId = tankID;
        isTankDead = true;
    }

    public void SpawnPlayerAtAvailablePoint(){
        SpawnScreen.SetActive(true);
        int randomIndex = Random.Range(0, spawnPoints.Length);
        int initialIndex = randomIndex;
        bool isOccupied = spawnPoints[randomIndex].GetComponent<SpawnPointChecker>().isOccupied;
        
        while (isOccupied){
            randomIndex = (randomIndex+1) % spawnPoints.Length;
            isOccupied = spawnPoints[randomIndex].GetComponent<SpawnPointChecker>().isOccupied;
            if(randomIndex==initialIndex){
                Debug.LogWarning("All spawm points occupied try later");
                return;
            }
        }
        SpawnScreen.SetActive(false);
        Vector3 randomPosition = spawnPoints[randomIndex].position;
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        // CameraManager.instance.BlendToTankCamera();
    }

    public void Spawn(){
        SpawnScreen.SetActive(false);
        SpawnPlayerAtAvailablePoint();
    }

    public void Respawn(){
        isTankDead=false;
        RespawnScreen.SetActive(false);
        SpawnPlayerAtAvailablePoint();
    }

    public void DeathScreenActive()
    {
        RespawnScreen.SetActive(true);
    }
}
