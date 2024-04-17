using UnityEngine;

public class BarrelCrosshair : MonoBehaviour
{
    public Transform tankBarrel;
    public float followSpeed = 10f;

    void Update()
    {
        if (tankBarrel != null)
        {
            // Smoothly move the crosshair towards the position of the barrel
            Vector3 targetPosition = tankBarrel.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
