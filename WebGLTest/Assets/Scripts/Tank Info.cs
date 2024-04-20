using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TankInfo : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank
    public GameObject destroyPrefab;
    public TMP_Text healthText; // Reference to the TextMeshPro component for displaying health
    public KeyCode repairKey = KeyCode.X; // Key to trigger repair
    public GameObject healthIndicator; // Reference to the game object to enable/disable

    private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
    public float repairTime = 7f; // Time in seconds for repair
    private float healAmountMin = 5f; // Minimum amount of healing
    private float healAmountMax = 15f; // Maximum amount of healing

    void Start()
    {
        // Initialize current health to max health at the start
        currentHealth = maxHealth;

        // Find the TextMeshPro component in the scene and assign it to the healthText field
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();

        // Update the health UI text
    }

    void Update()
    {
        UpdateHealthText();
        // Check if the repair key is pressed
        if (Input.GetKeyDown(repairKey))
        {
            // Start the repair process
            StartRepair();
        }
        else
        {
            isRepairing = false;
        }

        // If currently repairing, decrease repair time
        if (isRepairing)
        {
            repairTime -= Time.deltaTime;

            // Check if repair time is over
            if (repairTime <= 0f)
            {
                // End repair process
                EndRepair();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp health between 0 and maxHealth

        // Update the health UI text

        if (currentHealth <= 0)
        {
            // Perform actions when tank's health reaches zero
            DestructionPhase();
            Destroy(gameObject);
        }
    }

    public void StartRepair()
    {
        isRepairing = true;
        Debug.Log("Starting repair...");
    }

    public void EndRepair()
    {
        isRepairing = false;

        // Generate random healing amount
        float healAmount = Random.Range(healAmountMin, healAmountMax);

        // Clamp heal amount so it doesn't exceed maxHealth
        float potentialHealth = currentHealth + healAmount;
        currentHealth = Mathf.Clamp(potentialHealth, 0f, maxHealth);

        // Update the health UI text

        Debug.Log("Repair complete. Healed " + healAmount + " health.");
        repairTime = 7f;
        isRepairing = true;
    }

    public void DestructionPhase()
    {
        GameObject newDestroy = Instantiate(destroyPrefab, transform.position, transform.rotation);
        Destroy(newDestroy, 2.5f);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString("0");
        }

        // Check if health is below 60
        if (currentHealth < 60f)
        {
            // Enable the health indicator game object
            healthIndicator.SetActive(true);
        }
        else
        {
            // Disable the health indicator game object
            healthIndicator.SetActive(false);
        }
    }
}
