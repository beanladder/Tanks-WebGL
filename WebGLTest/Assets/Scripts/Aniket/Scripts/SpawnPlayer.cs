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
    public GameObject DeathScreen;
    public Transform[] spawnPoints;
    public bool isTankDead = false;
    private int deadTankID = -1; // Store the Photon ID of the killed tank
    PhotonView view;

     void Awake()
     {
         instance = this;
     }

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in SpawnPlayer script!");
            return;
        }
        // Select a random spawn point from the array
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 randomPosition = spawnPoints[randomIndex].position;
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (isTankDead)
        {
            TankDeathCheck(deadTankID);
        }
    }

    public void DeathScreenActive()
    {
        DeathScreen.SetActive(true);
    }

    public void RespawnPlayer()
    {
        DeathScreen.SetActive(false);
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 randomPosition = spawnPoints[randomIndex].position;
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }

    public void TankDeathCheck(int tankID)
    {
        Debug.Log($"Local Player Actor Number: {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($"Tank ID to Check: {tankID}");

        if (PhotonNetwork.LocalPlayer.ActorNumber == tankID)
        {
            Debug.Log("Tank should die");
            DeathScreenActive();
        }
        isTankDead = false;
    }

    public void SetDeadTankID(int id)
    {
        deadTankID = id;
        isTankDead = true;
    }
}
