using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRecoil : MonoBehaviour
{

    public static TankRecoil Instance;
    public float recoilAngle = 10f; // Angle of rotation during recoil
    public float recoilDuration = 0.2f; // Duration of the recoil animation

    private Quaternion originalRotation; // Original rotation of the tank body


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        originalRotation = transform.localRotation; // Store the original rotation
    }

    public void ApplyRecoil()
    {// Calculate the direction of the recoil rotation
        Vector3 recoilDirection = -transform.right; // Assuming the tank's forward direction is along the Z-axis

        // Play recoil animation
        LeanTween.rotateAroundLocal(gameObject, recoilDirection, recoilAngle, recoilDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => ReturnToOriginalRotation());
    }

    void ReturnToOriginalRotation()
    {
        // Reset rotation after recoil animation
        LeanTween.rotateLocal(gameObject, Vector3.zero, recoilDuration)
            .setEase(LeanTweenType.easeOutQuad);
    }
}
