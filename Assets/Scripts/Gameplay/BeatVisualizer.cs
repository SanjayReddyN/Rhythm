using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visualIndicator;
    [SerializeField] private float pulseScale = 0.2f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private bool useInterpolation = true;

    private Vector3 originalScale;
    private float currentAnimTime;
    private bool isAnimating;
    private float beatProgress = 0f;

    void Start()
    {
        if (visualIndicator == null)
        {
            Debug.LogError("BeatVisualizer: Missing visualIndicator reference!");
            enabled = false;
            return;
        }

        originalScale = visualIndicator.transform.localScale;
        SubscribeToBeatEvents();
    }

    private void SubscribeToBeatEvents()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat += HandleBeat;
        }
    }

    void Update()
    {
        if (!FMODManager.Instance?.musicInstance.isValid() ?? false)
            return;

        if (useInterpolation)
        {
            try
            {
                float timeSinceLastBeat = FMODManager.Instance.GetTimeSinceLastBeat();
                // Clamp to prevent invalid values
                timeSinceLastBeat = Mathf.Clamp01(timeSinceLastBeat);

                float sinValue = Mathf.Sin(timeSinceLastBeat * Mathf.PI * 2);
                // Clamp the scale calculation
                float scaleModifier = Mathf.Clamp(sinValue * pulseScale * 0.5f, -pulseScale, pulseScale);
                float scale = 1f + scaleModifier;

                // Only apply if scale is valid
                if (!float.IsNaN(scale))
                {
                    visualIndicator.transform.localScale = originalScale * scale;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"BeatVisualizer error: {e.Message}");
                visualIndicator.transform.localScale = originalScale;
            }
        }
        else
        {
            // Original discrete animation
            if (!isAnimating) return;

            currentAnimTime += Time.deltaTime;
            float progress = currentAnimTime / animationDuration;

            if (progress <= 1)
            {
                float scale = 1f + Mathf.Sin(progress * Mathf.PI) * pulseScale;
                visualIndicator.transform.localScale = originalScale * scale;
            }
            else
            {
                isAnimating = false;
                visualIndicator.transform.localScale = originalScale;
            }
        }
    }

    private void HandleBeat()
    {
        currentAnimTime = 0;
        isAnimating = true;
    }

    private void OnDestroy()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat -= HandleBeat;
        }
    }
}