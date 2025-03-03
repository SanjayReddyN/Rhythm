using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float beatInterval = 1f;
    private float lastMoveTime = 0f;
    private float moveLeeway = 0.2f;

    [SerializeField] private Tilemap groundTilemap; // Inner Room tilemap
    [SerializeField] private Tilemap wallTilemap;   // Wall tilemap

    private bool canMove = true;
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from player!");
            return;
        }

        // Snap to initial grid position
        Vector3Int startCell = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(startCell);
    }

    void FixedUpdate()
    {
        if (canMove && Time.time - lastMoveTime >= beatInterval - moveLeeway)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        Vector2 input = playerInput.GetMovementInput();

        if (input.magnitude > 0)
        {
            // Convert input to grid direction
            Vector3Int direction = new Vector3Int(
                Mathf.RoundToInt(input.x),
                Mathf.RoundToInt(input.y),
                0
            );

            // Get current grid position
            Vector3Int currentCell = groundTilemap.WorldToCell(transform.position);
            Vector3Int targetCell = currentCell + direction;

            Vector3 searchPosition = groundTilemap.GetCellCenterWorld(targetCell);
            Debug.Log($"Searching for instruments at grid position: {targetCell}, world position: {searchPosition}");
            // Check for instrument at target position
            Collider2D[] colliders = Physics2D.OverlapPointAll(searchPosition);
            Debug.Log($"Found {colliders.Length} colliders at target position");

            foreach (Collider2D collider in colliders)
            {
                Debug.Log($"Checking collider: {collider.gameObject.name}");
                if (collider.TryGetComponent<InstrumentController>(out InstrumentController instrument))
                {
                    Debug.Log($"Instrument found: {instrument.gameObject.name}, attempting activation");
                    instrument.Activate();
                    return; // Don't move into the instrument
                }
            }

            // Check if move is valid
            if (IsValidMove(targetCell))
            {
                // Move to center of target cell
                Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
                rb.MovePosition(targetPosition);

                lastMoveTime = Time.time;
                canMove = false;
                Invoke("EnableMovement", moveLeeway);

                // Debug visualization
                Debug.DrawLine(transform.position, targetPosition, Color.green, 0.5f);
            }
            else
            {
                // Debug visualization for invalid move
                Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
                Debug.DrawLine(transform.position, targetPosition, Color.red, 0.5f);
            }
        }
    }

    private bool IsValidMove(Vector3Int targetCell)
    {
        // Check if target is within ground tilemap and not blocked by wall
        return groundTilemap.HasTile(targetCell) && !wallTilemap.HasTile(targetCell);
    }

    private void EnableMovement()
    {
        canMove = true;
    }

    public void SetBeatInterval(float interval)
    {
        beatInterval = interval;
    }

    public Vector3Int GetGridPosition()
    {
        return groundTilemap.WorldToCell(transform.position);
    }
}