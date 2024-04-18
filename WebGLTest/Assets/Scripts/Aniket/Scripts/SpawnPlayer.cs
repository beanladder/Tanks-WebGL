using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public float minX;
    public float maxX;
    public float fixedY;
    private GameObject player;
    void Start(){
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), fixedY);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        SetPlayerDisplayName(player);
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
}
