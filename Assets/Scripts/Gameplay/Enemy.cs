using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveInterval = 3f;
    [SerializeField] private float beatDuration = 1f;
    [SerializeField] private Tilemap groundTilemap;

    private float nextMoveTime;
    private Vector3Int currentGridPosition;
    private Rigidbody2D rb;

    // Add public setter for groundTilemap
    public void SetGroundTilemap(Tilemap tilemap)
    {
        groundTilemap = tilemap;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Enemy: Missing Rigidbody2D!");
            return;
        }

        // Find tilemap if not assigned
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();
        }

        nextMoveTime = Time.time + moveInterval * beatDuration;

        // Snap to grid on spawn
        currentGridPosition = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPosition);

        Debug.Log($"Enemy spawned at grid position: {currentGridPosition}");
    }

    private void Update()
    {
        if (Time.time >= nextMoveTime)
        {
            MoveTowardsPlayer();
            nextMoveTime = Time.time + moveInterval * beatDuration;
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