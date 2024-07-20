using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class HUDManager : MonoBehaviourPunCallbacks
{
    public GameObject hitmarker;
    public GameObject killmarker;
    //public Text killCountText;
    public GameObject hudCanvas;

    private int killCount = 0;
    private PhotonView photonViewID;

    void Start()
    {
        photonViewID = GetComponent<PhotonView>();
        if (!photonViewID.IsMine)
        {
            hudCanvas.SetActive(false);
        }
    }

    [PunRPC]
    public void ShowHitmarker()
    {
        if (photonViewID.IsMine)
        {
            StartCoroutine(ShowMarker(hitmarker, 1.7f));
        }
    }

    [PunRPC]
    public void ShowKillmarker()
    {
        if (photonViewID.IsMine)
        {
            StartCoroutine(ShowMarker(killmarker, 2f));
            killCount++;
            //UpdateKillCount();
        }
    }

    private IEnumerator ShowMarker(GameObject marker, float duration)
    {
        marker.SetActive(true);
        yield return new WaitForSeconds(duration);
        marker.SetActive(false);
    }

    /*private void UpdateKillCount()
    {
        killCountText.text = "Kills: " + killCount;
    }*/
}
