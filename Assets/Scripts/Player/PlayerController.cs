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
    private bool hasMovedThisBeat = false;
    private bool hasInteractedThisBeat = false;

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
        hasMovedThisBeat = false;
        hasInteractedThisBeat = false;
        canMove = true;
    }

    private void Update()
    {
        if (!canMove) return;

        float timeSinceLastBeat = FMODManager.Instance.GetTimeSinceLastBeat();
        bool isWithinBeatWindow = timeSinceLastBeat <= moveWindow || timeSinceLastBeat >= (1 - moveWindow);

        if (!isWithinBeatWindow)
        {
            canMove = false;
            return;
        }

        if (!hasMovedThisBeat)
        {
            Vector2 input = playerInput.GetMovementInput();
            if (input.magnitude > 0)
            {
                TryMove(input);
            }
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

        // Check for instrument at target position
        if (TryInteractWithInstrumentAt(targetCell))
        {
            hasMovedThisBeat = true;
            canMove = false;
            return;
        }

        // If no instrument, try to move
        if (IsValidMove(targetCell))
        {
            Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
            rb.MovePosition(targetPosition);
            lastPosition = targetCell;
            hasMovedThisBeat = true;
            canMove = false;
        }
    }

    private bool TryInteractWithInstrumentAt(Vector3Int targetCell)
    {
        Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);
        Collider2D[] colliders = Physics2D.OverlapPointAll(targetPosition);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<InstrumentController>(out var instrument))
            {
                instrument.Activate();
                return true;
            }
        }
        return false;
    }

    private bool IsValidMove(Vector3Int targetCell)
    {
        return groundTilemap.HasTile(targetCell) && !wallTilemap.HasTile(targetCell);
    }

    private void OnDestroy()
    {
        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat -= HandleBeat;
        }
    }
}