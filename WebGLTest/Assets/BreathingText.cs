using System.Collections;
using UnityEngine;
using TMPro;

public class BreathingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;  // Reference to your TextMeshProUGUI component
    public float scaleFactor = 1.2f;     // How much to scale up
    public float duration = 1.0f;        // Duration of one breath cycle

    private Vector3 originalScale;

    private void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro component is not assigned.");
            return;
        }

        originalScale = textMeshPro.transform.localScale;
        StartCoroutine(BreathingEffect());
    }

    private IEnumerator BreathingEffect()
    {
        while (true)
        {
            // Scale up
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float scale = Mathf.Lerp(1f, scaleFactor, t);
                textMeshPro.transform.localScale = originalScale * scale;
                yield return null;
            }

            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float scale = Mathf.Lerp(scaleFactor, 1f, t);
                textMeshPro.transform.localScale = originalScale * scale;
                yield return null;
            }
        }
    }
}
