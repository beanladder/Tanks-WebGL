using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineFreeLook mapCamera;
    private CinemachineFreeLook currentTankCamera;

    public CinemachineBrain cinemachineBrain;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentTankCamera(CinemachineFreeLook tankCamera)
    {
        currentTankCamera = tankCamera;
    }

    public void BlendToMapCamera()
    {
        if (currentTankCamera != null && mapCamera != null)
        {
            currentTankCamera.Priority = 0;
            mapCamera.Priority = 10;
        }
    }

    public void BlendToTankCamera()
    {
        if (currentTankCamera != null && mapCamera != null)
        {
            mapCamera.Priority = 0;
            currentTankCamera.Priority = 10;
        }
    }
}
