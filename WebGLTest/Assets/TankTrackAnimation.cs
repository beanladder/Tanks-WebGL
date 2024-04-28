using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrackAnimation : MonoBehaviour
{
    public Renderer trackRenderer;
    public float scrollSpeed = 1.0f;

    private Material trackMaterial;

    void Start()
    {
        trackMaterial = trackRenderer.material;
    }

    void Update()
    {
        // Adjust the speed of the animation over time
        float speed = Mathf.Sin(Time.time) * scrollSpeed;
        trackMaterial.SetFloat("_Speed", speed);
    }
}
