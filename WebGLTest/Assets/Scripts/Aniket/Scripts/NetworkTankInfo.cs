using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkTankInfo : MonoBehaviour
{

    public GameObject Tank; 
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank
    void Start()
    {
        // Initialize current health to max health at the start
        currentHealth = maxHealth;
    }

    void Update()
    {
        if(currentHealth<1)
        {
            Destroy(Tank);
        }
    }

    public void TakeDamage(int Damage)
    {
        currentHealth = currentHealth - Damage;

    }

    public void Repair(int heal)
    {
        currentHealth = currentHealth + heal;

    }


}
