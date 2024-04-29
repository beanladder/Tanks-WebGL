using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformChecker : MonoBehaviour
{
    
    public GameObject mobileControls;

    void Start()
    {
        // Check if the platform is mobile
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Activate mobile controls
            mobileControls.SetActive(true);
            
        }
        else
        {
            // Activate PC controls
            mobileControls.SetActive(false);
            
        }
    }
}
