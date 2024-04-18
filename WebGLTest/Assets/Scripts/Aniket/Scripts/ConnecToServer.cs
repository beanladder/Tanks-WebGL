using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject loadingUI;
    public GameObject namingUI;
    public InputField displayNameInput;

    // Start is called before the first frame update
    void Start()
    {
        namingUI.SetActive(true);
        loadingUI.SetActive(false);
    }

    // Update is called once per frame
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void StartGame(){
        string displayName = displayNameInput.text;
        PhotonNetwork.NickName = displayName;
        namingUI.SetActive(false);
        loadingUI.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }
}
