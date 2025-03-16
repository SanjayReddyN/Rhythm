using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class InstrumentController : MonoBehaviour
{
    [SerializeField] private InstrumentType instrumentType;
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private AudioSource instrumentSound;
    [SerializeField] private Direction facing;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private float attackDelay = 0.1f; // Small delay for visual feedback
    [SerializeField] private TimingFeedback timingFeedback;

    public enum InstrumentType
    {
        Piano,
        Drums,
        Guitar
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    private Vector2 attackDirection;

    private void Start()
    {
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();
        }

        // Snap to grid
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(cellPosition);

        Debug.Log($"Instrument snapped to grid position: {cellPosition}");

        // Set attack direction based on facing
        switch (facing)
        {
            case Direction.North:
                attackDirection = Vector2.up;
                break;
            case Direction.South:
                attackDirection = Vector2.down;
                break;
            case Direction.East:
                attackDirection = Vector2.right;
                break;
            case Direction.West:
                attackDirection = Vector2.left;
                break;
        }

        if (timingFeedback == null)
        {
            timingFeedback = FindFirstObjectByType<TimingFeedback>();
        }
    }

    public void Activate()
    {
        float timingSinceBeat = FMODManager.Instance.GetTimeSinceLastBeat();

        // Show timing feedback
        if (timingFeedback != null)
        {
            timingFeedback.ShowFeedback(timingSinceBeat, transform.position + Vector3.up);
        }

        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        // Visual feedback first
        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        if (instrumentSound != null)
        {
            instrumentSound.Play();
        }

        // Small delay before damage
        yield return new WaitForSeconds(attackDelay);

        // Check for enemies
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            attackDirection,
            attackRange,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage();
            }
        }

        // Debug visuals
        Debug.DrawRay(transform.position, attackDirection * attackRange, Color.red, 0.5f);
    }

    // Helper method to draw debug circle
    private void DebugDrawCircle(Vector3 center, float radius, Color color, float duration)
    {
        int segments = 20;
        float angle = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float currentAngle = angle * i * Mathf.Deg2Rad;
            float nextAngle = angle * (i + 1) * Mathf.Deg2Rad;
            Vector3 currentPoint = center + new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;
            Debug.DrawLine(currentPoint, nextPoint, color, duration);
        }
    }
}