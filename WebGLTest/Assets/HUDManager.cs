using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class HUDManager : MonoBehaviour
{
    public GameObject hitMarker;
    public GameObject killMarker;

    public void ShowHitmarker()
    {
        StartCoroutine(ShowMarker(hitMarker));
    }

    public void ShowKillmarker()
    {
        StartCoroutine(ShowMarker(killMarker));
    }

    private IEnumerator ShowMarker(GameObject marker)
    {
        marker.SetActive(true);
        yield return new WaitForSeconds(0.5f); // Display marker for 0.5 seconds
        marker.SetActive(false);
    }
}