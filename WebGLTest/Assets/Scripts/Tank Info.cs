using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInfo : MonoBehaviour
{
    
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank

    void Start()
    {
        // Initialize current health to max health at the start
        currentHealth = maxHealth;
    }

    
}
