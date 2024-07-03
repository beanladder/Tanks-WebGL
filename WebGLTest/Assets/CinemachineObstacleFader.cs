using UnityEngine;
using Cinemachine;

public class CinemachineObstacleFader : MonoBehaviour
{
    public Transform player;
    public float fadeSpeed = 2f;
    public LayerMask obstacleLayer; // Layer mask to filter obstacles

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (cam != null && player != null)
        {
            Vector3 cameraPosition = cam.transform.position;
            Vector3 playerPosition = player.position;
            Vector3 direction = playerPosition - cameraPosition;

            // Visualize the line of sight in the Scene view
            Debug.DrawLine(cameraPosition, playerPosition, Color.blue);

            // Perform a raycast to find all obstacles in the line of sight
            RaycastHit[] hits = Physics.RaycastAll(cameraPosition, direction, Mathf.Infinity, obstacleLayer);

            foreach (RaycastHit hit in hits)
            {
                Renderer obstacleRenderer = hit.collider.gameObject.GetComponent<Renderer>();
                if (obstacleRenderer != null)
                {
                    FadeObstacle(obstacleRenderer);
                }
            }
        }
    }

    void FadeObstacle(Renderer renderer)
    {
        Color currentColor = renderer.material.color;
        Color fadedColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0.3f);
        renderer.material.color = Color.Lerp(currentColor, fadedColor, fadeSpeed * Time.deltaTime);
    }
}
