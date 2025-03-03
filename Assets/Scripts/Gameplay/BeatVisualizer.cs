using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visualIndicator;
    private float beatInterval = 1f;
    private float currentTime = 0f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = visualIndicator.transform.localScale;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        float beatProgress = (currentTime % beatInterval) / beatInterval;
        float scale = 1f + Mathf.Sin(beatProgress * Mathf.PI * 2) * 0.2f;
        visualIndicator.transform.localScale = originalScale * scale;
    }

    public void SetBeatInterval(float interval)
    {
        beatInterval = interval;
    }
}