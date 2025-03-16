using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int moveEveryNBeats = 2;
    [SerializeField] private Tilemap groundTilemap;

    private int beatCounter = 0;
    private Vector3Int currentGridPosition;
    private Rigidbody2D rb;

    // Add public setter for groundTilemap
    public void SetGroundTilemap(Tilemap tilemap)
    {
        groundTilemap = tilemap;
    }

    private void Start()
    {
        InitializeComponents();
        SubscribeToBeatEvents();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();
        }

        // Snap to grid on spawn
        currentGridPosition = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPosition);
    }

    private void SubscribeToBeatEvents()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat += HandleBeat;
        }
    }

    private void HandleBeat()
    {
        beatCounter++;
        if (beatCounter >= moveEveryNBeats)
        {
            MoveTowardsPlayer();
            beatCounter = 0;
        }
    }

    private void OnDestroy()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat -= HandleBeat;
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calculate next grid position
        Vector3Int targetGridPosition = currentGridPosition + new Vector3Int(0, 1, 0);

        // Check if move is valid (tile exists)
        if (groundTilemap.HasTile(targetGridPosition))
        {
            Debug.Log($"Enemy moving from {currentGridPosition} to {targetGridPosition}");
            currentGridPosition = targetGridPosition;
            Vector3 targetWorldPosition = groundTilemap.GetCellCenterWorld(targetGridPosition);
            rb.MovePosition(targetWorldPosition);
        }
        else
        {
            Debug.LogWarning($"Enemy attempted invalid move to {targetGridPosition}");
        }
    }

    public void TakeDamage()
    {
        Debug.Log($"Enemy eliminated at position {currentGridPosition}");
        Destroy(gameObject);
    }
}