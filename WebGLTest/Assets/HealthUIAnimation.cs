// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class HealthUIAnimation : MonoBehaviour
// {
//     public static HealthUIAnimation instance;
//     public Transform gearImage; // Reference to the gear image transform
//     public float rotationSpeed = 180f; // Speed of rotation

//     private Vector3 originalScale; // Original scale of the gear image
//     private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
//     private Coroutine repairCoroutine; // Reference to the repair coroutine


//     private void Awake()
//     {
//         instance = this;
//     }
//     void Start()
//     {
//         // Store the original scale of the gear image
//         originalScale = gearImage.localScale;
//     }

//     void Update()
//     {
      
//     }

//     public void StartRepairAnimation()
//     {
//         //if (!isRepairing)
//         //{
//         //    isRepairing = true;
//             repairCoroutine = StartCoroutine(RepairAnimation());
//         //}
//     }

//     public void StopRepairAnimation()
//     {
//         //if (isRepairing && repairCoroutine != null)
//         //{
//             StopCoroutine(repairCoroutine);
//             isRepairing = false;
//             // Reset the gear image to its original state
//             gearImage.localScale = originalScale;
//         //}
//     }

//     public IEnumerator RepairAnimation()
//     {
//         // Increase scale once
//         gearImage.localScale = originalScale * 1.15f;

//         // Rotate the gear image continuously
//         while (true)
//         {
//             gearImage.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
//             yield return null;
//         }
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIAnimation : MonoBehaviour
{
    public static HealthUIAnimation instance;
    public Transform gearImage; // Reference to the gear image transform
    public float rotationSpeed = 180f; // Speed of rotation

    private Vector3 originalScale; // Original scale of the gear image
    private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
    private Coroutine repairCoroutine; // Reference to the repair coroutine

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    void Start()
    {
        if (gearImage == null)
        {
            Debug.LogError("Gear Image is not assigned in the Inspector");
            return;
        }

        // Store the original scale of the gear image
        originalScale = gearImage.localScale;
    }

    public void StartRepairAnimation()
    {
        if (!isRepairing)
        {
            isRepairing = true;
            repairCoroutine = StartCoroutine(RepairAnimation());
        }
    }

    public void StopRepairAnimation()
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
        while (isRepairing)
        {
            gearImage.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
