using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Dictionary<Collider, List<MeshRenderer>> disabledRenderers = new Dictionary<Collider, List<MeshRenderer>>();

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            enabled = false;
            return;
        }

        // Add a collider to the camera if it doesn't have one
        if (mainCamera.GetComponent<Collider>() == null)
        {
            mainCamera.gameObject.AddComponent<SphereCollider>().isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!disabledRenderers.ContainsKey(other))
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();

            // Get the MeshRenderer of the collided object
            MeshRenderer mainRenderer = other.GetComponent<MeshRenderer>();
            if (mainRenderer != null)
            {
                renderers.Add(mainRenderer);
                mainRenderer.enabled = false;
            }

            // Get MeshRenderers of all children
            MeshRenderer[] childRenderers = other.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer childRenderer in childRenderers)
            {
                if (!renderers.Contains(childRenderer))
                {
                    renderers.Add(childRenderer);
                    childRenderer.enabled = false;
                }
            }

            if (renderers.Count > 0)
            {
                disabledRenderers[other] = renderers;
                Debug.Log($"Disabled renderers for {other.gameObject.name}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (disabledRenderers.TryGetValue(other, out List<MeshRenderer> renderers))
        {
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
            disabledRenderers.Remove(other);
            Debug.Log($"Enabled renderers for {other.gameObject.name}");
        }
    }
}
