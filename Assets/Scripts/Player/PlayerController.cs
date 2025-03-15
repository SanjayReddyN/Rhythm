using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Beat Settings")]
    [SerializeField] private float moveWindow = 0.2f;
    [SerializeField] private float beatTolerance = 0.05f;

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private float lastBeatTime;
    private bool canMove;
    private Vector3Int lastPosition;

    private void Start()
    {
        InitializeComponents();
        SubscribeToBeatEvents();
        SnapToGrid();
    }

    private void InitializeComponents()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null || groundTilemap == null || wallTilemap == null)
        {
            Debug.LogError("PlayerController: Missing required components!");
            enabled = false;
        }
    }

    private void SubscribeToBeatEvents()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat += HandleBeat;
        }
    }

    private void SnapToGrid()
    {
        lastPosition = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(lastPosition);
    }

    private void HandleBeat()
    {
        lastBeatTime = Time.time;
        canMove = true;
    }

    private void Update()
    {
        if (!canMove) return;

        float timeSinceLastBeat = Time.time - lastBeatTime;
        if (timeSinceLastBeat > moveWindow + beatTolerance)
        {
            canMove = false;
            return;
        }

        Vector2 input = playerInput.GetMovementInput();
        if (input.magnitude > 0)
        {
            TryMove(input);
        }
    }

    private void TryMove(Vector2 input)
    {
        Vector3Int direction = new Vector3Int(
            Mathf.RoundToInt(input.x),
            Mathf.RoundToInt(input.y),
            0
        );

        // Prevent diagonal movement
        if (direction.x != 0) direction.y = 0;

        Vector3Int targetCell = lastPosition + direction;

        if (IsValidMove(targetCell))
        {
            Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
            rb.MovePosition(targetPosition);
            lastPosition = targetCell;
            canMove = false; // Prevent multiple moves per beat
        }
    }

    private bool IsValidMove(Vector3Int targetCell)
    {
        if (!groundTilemap.HasTile(targetCell) || wallTilemap.HasTile(targetCell))
            return false;

        // Check for interactables
        Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
        Collider2D[] colliders = Physics2D.OverlapPointAll(targetPosition);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<InstrumentController>(out var instrument))
            {
                instrument.Activate();
                return false; // Don't move into instrument spaces
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat -= HandleBeat;
        }
    }
}