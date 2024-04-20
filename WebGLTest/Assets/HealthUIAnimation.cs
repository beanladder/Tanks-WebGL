using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIAnimation : MonoBehaviour
{
    public Transform gearImage; // Reference to the gear image transform
    public float animationSpeed = 1f; // Speed of the animation
    public float rotationSpeed = 180f; // Speed of rotation

    private Vector3 originalScale; // Original scale of the gear image
    private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
    private Coroutine repairCoroutine; // Reference to the repair coroutine

    void Start()
    {
        // Store the original scale of the gear image
        originalScale = gearImage.localScale;
    }

    void Update()
    {
        // Check if the repair key is pressed
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Start the repair animation
            StartRepairAnimation();
        }

        // Check if the repair key is released
        if (Input.GetKeyUp(KeyCode.X))
        {
            // Stop the repair animation
            StopRepairAnimation();
        }
    }

    private void StartRepairAnimation()
    {
        if (!isRepairing)
        {
            isRepairing = true;
            repairCoroutine = StartCoroutine(RepairAnimation());
        }
    }

    private void StopRepairAnimation()
    {
        if (isRepairing && repairCoroutine != null)
        {
            StopCoroutine(repairCoroutine);
            isRepairing = false;
            // Reset the gear image to its original state
            gearImage.localScale = originalScale;
        }
    }

    private IEnumerator RepairAnimation()
    {
        // Increase scale once
        gearImage.localScale = originalScale * 1.15f;

        // Rotate the gear image continuously
        while (true)
        {
            gearImage.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
