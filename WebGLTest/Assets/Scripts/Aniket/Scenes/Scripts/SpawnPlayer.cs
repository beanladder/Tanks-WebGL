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

    [SerializeField]private GameObject newPlayer;


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




    public void SetDeadTankId(int tankID)
    {
        deadTankId = tankID;
        isTankDead = true;
        StartCoroutine(DelayedRespawnScreenActivation(2.2f));
    }

    private IEnumerator DelayedRespawnScreenActivation(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnScreen.SetActive(true);
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
        newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        SetPlayerLayer(newPlayer);
        StartCoroutine(FadeInUICanvas());
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
    private IEnumerator FadeInUICanvas()
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("Player instance is not yet created.");
            yield break;
        }

        CanvasGroup uiCanvasGroup = newPlayer.GetComponentInChildren<CanvasGroup>();
        if (uiCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found in player instance.");
            yield break;
        }

        float duration = 1.7f; // Total fade-in duration
        float elapsedTime = 0f;

        // Initially, set the alpha to 0
        uiCanvasGroup.alpha = 0f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            uiCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Ensure the alpha is set to 1 at the end
        uiCanvasGroup.alpha = 1f;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;
    }


}
