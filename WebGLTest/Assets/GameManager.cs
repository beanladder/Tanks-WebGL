using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isInteractableUI;

    private void Awake()
    {
        // Ensure this instance is not destroyed when loading new scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Start the coroutine to log cursor state and interactive UI every 2 seconds
        StartCoroutine(DebugCursorStateAndUI());
    }

    private void Update()
    {
        // Check if there's any active interactive UI element in the scene
        bool newInteractableUI = CheckForActiveInteractiveUI();

        // Update cursor state if interactable UI state changes
        if (isInteractableUI != newInteractableUI)
        {
            isInteractableUI = newInteractableUI;
            UpdateCursorState();
        }
    }

    private bool CheckForActiveInteractiveUI()
    {
        // Find all canvases in the scene
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            // Check if the canvas has a GraphicRaycaster component and is enabled
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null && raycaster.enabled)
            {
                // Check if the canvas has any interactive UI elements
                if (HasInteractiveUIElements(canvas.transform))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HasInteractiveUIElements(Transform parentTransform)
    {
        // Check for interactive elements within the given transform
        foreach (Transform child in parentTransform)
        {
            if (child.GetComponent<Button>() != null || child.GetComponent<InputField>() != null ||
                child.GetComponent<Toggle>() != null || child.GetComponent<Dropdown>() != null)
            {
                // Found an interactive element
                return true;
            }

            // Recursively check child objects
            if (HasInteractiveUIElements(child))
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateCursorState()
    {
        if (isInteractableUI)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // Debug logging to track cursor state changes
        Debug.Log($"Cursor State - Locked: {Cursor.lockState}, Visible: {Cursor.visible}");
    }

    private IEnumerator DebugCursorStateAndUI()
    {
        while (true)
        {
            // Log the cursor state
            Debug.Log($"Cursor State - Locked: {Cursor.lockState}, Visible: {Cursor.visible}");

            // Log the interactive UI state and canvas names
            LogInteractiveUIState();

            yield return new WaitForSeconds(2f);
        }
    }

    private void LogInteractiveUIState()
    {
        // Find all canvases in the scene
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        bool hasInteractiveUI = false;

        foreach (var canvas in canvases)
        {
            // Check if the canvas has a GraphicRaycaster component and is enabled
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null && raycaster.enabled)
            {
                // Check if the canvas has any interactive UI elements
                if (HasInteractiveUIElements(canvas.transform))
                {
                    hasInteractiveUI = true;
                    Debug.Log($"Interactive UI found on Canvas: {canvas.name}");
                }
            }
        }

        if (!hasInteractiveUI)
        {
            Debug.Log("No interactive UI found.");
        }
    }
}
