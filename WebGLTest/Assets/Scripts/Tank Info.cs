using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInfo : MonoBehaviour
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
            //Destroy(Tank);
        }
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -=Damage;

    }

    public void Repair(int heal)
    {
        if(currentHealth<maxHealth){
            currentHealth = currentHealth + heal;

        }

    }
}
