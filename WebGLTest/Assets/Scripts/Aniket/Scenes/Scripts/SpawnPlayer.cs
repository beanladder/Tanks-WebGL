using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public static SpawnPlayer instance;
    public GameObject playerPrefab;
    private GameObject player;
    public GameObject SpawnScreen;
    public GameObject RespawnScreen;
    public Transform[] spawnPoints;
    public string playerName;
    public float sphereRadius = 10f;
    private int deadTankId = -1;
    private bool isTankDead = false;
    private PhotonView view;
    public string localPlayerLayerName = "Tank";
    

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
    }

    void Update()
    {
        if (isTankDead && PhotonNetwork.LocalPlayer.ActorNumber == deadTankId)
        {
            RespawnScreen.SetActive(true);
        }
        else
        {
            RespawnScreen.SetActive(false);
        }
    }

    public void SetDeadTankId(int tankID)
    {
        deadTankId = tankID;
        isTankDead = true;
    }

    public void SpawnPlayerAtAvailablePoint()
    {
        SpawnScreen.SetActive(true);
        int randomIndex = Random.Range(0, spawnPoints.Length);
        int initialIndex = randomIndex;
        bool isOccupied = spawnPoints[randomIndex].GetComponent<SpawnPointChecker>().isOccupied;

        while (isOccupied)
        {
            randomIndex = (randomIndex + 1) % spawnPoints.Length;
            isOccupied = spawnPoints[randomIndex].GetComponent<SpawnPointChecker>().isOccupied;
            if (randomIndex == initialIndex)
            {
                Debug.LogWarning("All spawn points occupied, try later");
                return;
            }
        }
        SpawnScreen.SetActive(false);
        Vector3 randomPosition = spawnPoints[randomIndex].position;
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        SetPlayerLayer(newPlayer);
    }

    public void Spawn()
    {
        SpawnScreen.SetActive(false);
        SpawnPlayerAtAvailablePoint();
    }

    public void Respawn()
    {
        isTankDead = false;
        RespawnScreen.SetActive(false);
        SpawnPlayerAtAvailablePoint();
    }

    public void DeathScreenActive()
    {
        RespawnScreen.SetActive(true);
    }

    private void SetPlayerLayer(GameObject player)
    {
        PhotonView photonView = player.GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            SetLayerRecursively(player, LayerMask.NameToLayer(localPlayerLayerName));
        }
        
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
