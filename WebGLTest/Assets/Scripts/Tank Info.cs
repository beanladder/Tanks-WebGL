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

    void Update()
    {
        if (currentHealth < 1)
        {
            // Destroy tank or perform other actions when health reaches zero
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Damage Inflicted: " + damage);

        if (currentHealth < 1)
        {
            // Perform actions when tank's health reaches zero
            Destroy(gameObject);
        }
    }

    public void Repair(int heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
        }
    }
}

