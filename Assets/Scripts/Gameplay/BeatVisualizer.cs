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
        if (useInterpolation)
        {
            // Smoothly animate between beats
            float timeSinceLastBeat = FMODManager.Instance.GetTimeSinceLastBeat();
            beatProgress = timeSinceLastBeat;

            float scale = 1f + Mathf.Sin(beatProgress * Mathf.PI * 2) * pulseScale * 0.5f;
            visualIndicator.transform.localScale = originalScale * scale;
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