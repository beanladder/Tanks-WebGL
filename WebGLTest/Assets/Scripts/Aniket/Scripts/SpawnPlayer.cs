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
    public GameObject textPrefab;
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
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        //if(player.GetComponent<PhotonView>().IsMine){
            GameObject textObject = Instantiate(textPrefab, player.transform);
            TextMeshProUGUI textMesh = textObject.GetComponent<TextMeshProUGUI>();
            if(textMesh!=null){
                textMesh.text = PhotonNetwork.NickName;
            }
        //}
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
