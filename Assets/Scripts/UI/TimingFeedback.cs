using UnityEngine;
using TMPro;

public class TimingFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI staticTimingText;
    [SerializeField] private TextMeshProUGUI feedbackPrefab;
    [SerializeField] private Transform worldSpaceFeedback;

    [Header("Timing Settings")]
    [SerializeField] private float perfectThreshold = 0.05f;
    [SerializeField] private float goodThreshold = 0.1f;

    [Header("Animation")]
    [SerializeField] private float displayDuration = 0.5f;
    [SerializeField] private float floatDistance = 50f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Colors")]
    [SerializeField] private Color perfectColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color goodColor = new Color(1f, 1f, 0.2f);
    [SerializeField] private Color missColor = new Color(1f, 0.2f, 0.2f);

    private void Start()
    {
        if (staticTimingText != null)
        {
            staticTimingText.text = "";
        }
    }

    public void ShowFeedback(float accuracy, Vector3 worldPosition)
    {
        // Update static timing text
        UpdateStaticTimingText(accuracy);

        // Spawn world space feedback
        SpawnWorldFeedback(accuracy, worldPosition);
    }

    private void UpdateStaticTimingText(float accuracy)
    {
        if (staticTimingText == null) return;

        string text;
        Color color;

        if (accuracy <= perfectThreshold)
        {
            text = "PERFECT!";
            color = perfectColor;
        }
        else if (accuracy <= goodThreshold)
        {
            text = "GOOD";
            color = goodColor;
        }
        else
        {
            text = "MISS";
            color = missColor;
        }

        staticTimingText.text = text;
        staticTimingText.color = color;

        // Reset text after delay
        CancelInvoke(nameof(ClearStaticText));
        Invoke(nameof(ClearStaticText), displayDuration);
    }

    private void SpawnWorldFeedback(float accuracy, Vector3 worldPosition)
    {
        if (feedbackPrefab == null || worldSpaceFeedback == null) return;

        TextMeshProUGUI feedback = Instantiate(feedbackPrefab, worldSpaceFeedback);

        // Set text and color based on accuracy
        if (accuracy <= perfectThreshold)
        {
            feedback.text = "PERFECT!";
            feedback.color = perfectColor;
        }
        else if (accuracy <= goodThreshold)
        {
            feedback.text = "GOOD";
            feedback.color = goodColor;
        }
        else
        {
            feedback.text = "MISS";
            feedback.color = missColor;
        }

        // Position in world space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        feedback.transform.position = screenPos;

        // Start animation coroutine
        StartCoroutine(AnimateFeedback(feedback));
    }

    private System.Collections.IEnumerator AnimateFeedback(TextMeshProUGUI feedback)
    {
        float startTime = Time.time;
        Vector3 startPos = feedback.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;
        Color startColor = feedback.color;

        while (Time.time - startTime < displayDuration)
        {
            float t = (Time.time - startTime) / displayDuration;

            // Position
            feedback.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Fade
            Color newColor = startColor;
            newColor.a = fadeCurve.Evaluate(t);
            feedback.color = newColor;

            yield return null;
        }

        Destroy(feedback.gameObject);
    }

    private void ClearStaticText()
    {
        if (staticTimingText != null)
        {
            staticTimingText.text = "";
        }
    }
}