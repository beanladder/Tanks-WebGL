using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class HUDManager : MonoBehaviourPunCallbacks
{
    public GameObject hitmarker;
    public GameObject killmarker;
    public GameObject hudCanvas;

    void Start()
    {
        if (!photonView.IsMine)
        {
            hudCanvas.SetActive(false);
        }
    }

    [PunRPC]
    public void ShowHitmarker(Vector3 impactPosition, Vector3 impulseDirection, float impulseForce)
    {
        if (photonView.IsMine)
        {
            StartCoroutine(ShowMarker(hitmarker, 0.5f));
        }
    }

    [PunRPC]
    public void ShowKillmarker()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(ShowMarker(killmarker, 1f));
        }
    }

    private IEnumerator ShowMarker(GameObject marker, float duration)
    {
        marker.SetActive(true);
        yield return new WaitForSeconds(duration);
        marker.SetActive(false);
    }
}
