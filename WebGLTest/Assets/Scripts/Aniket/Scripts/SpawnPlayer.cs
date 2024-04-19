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
    public float minX;
    public float maxX;
    public float fixedY;
    private GameObject player;
    public GameObject DeathScreen;
    private bool isTankDead = false;
    private int deadTankID = -1; // Store the Photon ID of the killed tank
    PhotonView view;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), fixedY);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        SetPlayerDisplayName(player);
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (isTankDead)
        {
            TankDeathCheck(deadTankID);
        }
    }

    void SetPlayerDisplayName(GameObject player)
    {
        // Retrieve the display name of the player from PhotonNetwork
        string displayName = PhotonNetwork.NickName;

        // Set the display name of the player using PhotonView
        PhotonView photonView = player.GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.RPC("SetDisplayNameRPC", RpcTarget.AllBuffered, displayName);
        }
    }

    // RPC method to set the display name of the player on all clients
    [PunRPC]
    void SetDisplayNameRPC(string displayName)
    {
        // Set the display name of the player
        gameObject.name = displayName;
    }

    public void DeathScreenActive()
    {
        DeathScreen.SetActive(true);
    }

    public void RespawnPlayer()
    {
        DeathScreen.SetActive(false);
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), fixedY);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }

    public void TankDeathCheck(int tankID)
    {
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
