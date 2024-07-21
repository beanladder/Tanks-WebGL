using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class HUDManager : MonoBehaviour
{
    public GameObject hitMarker;
    public GameObject killMarker;
    private float hitMarkerDisplayTime = 2f; // Duration for displaying hit marker
    private float killMarkerDisplayTime = 3f; // Duration for displaying kill marker

    public void ShowHitmarker()
    {
        StartCoroutine(ShowMarker(hitMarker, hitMarkerDisplayTime));
    }

    public void ShowKillmarker()
    {
        StartCoroutine(ShowMarker(killMarker, killMarkerDisplayTime));
    }

    private IEnumerator ShowMarker(GameObject marker, float displayTime)
    {
        marker.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        marker.SetActive(false);
    }
}
