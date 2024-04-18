using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Deactivate : MonoBehaviour
{
    private PhotonView viewCamera;
    // Start is called before the first frame update
    void Start()
    {
        viewCamera = GetComponent<PhotonView>();
        if(!viewCamera.IsMine){
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
