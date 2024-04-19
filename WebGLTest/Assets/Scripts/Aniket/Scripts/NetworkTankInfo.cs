using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkTankInfo : MonoBehaviour
{
    public GameObject Tank;
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank
    PhotonView view;

    void Start()
    {
        // Initialize current health to max health at the start
        currentHealth = maxHealth;
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (currentHealth < 1)
        {
            SpawnPlayer.instance.SetDeadTankID(view.OwnerActorNr); // Pass the Photon ID of the killed tank
            Destroy(Tank, 0.5f);
            Cursor.lockState= CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    [PunRPC]
    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage; // Simplified subtraction
        currentHealth = Mathf.Max(currentHealth, 0f);
        view.RPC("SyncHealth", RpcTarget.AllBuffered, currentHealth);
    }

    public void Repair(int heal)
    {
        currentHealth += heal; // Simplified addition
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        view.RPC("SyncHealth", RpcTarget.AllBuffered, currentHealth);
    }

    [PunRPC]
    void SyncHealth(float health)
    {
        currentHealth = health;
    }
}
