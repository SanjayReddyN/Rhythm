using UnityEngine;
using UnityEngine.Tilemaps;

public class InstrumentController : MonoBehaviour
{
    [SerializeField] private InstrumentType instrumentType;
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private AudioSource instrumentSound;
    [SerializeField] private Direction facing;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private Tilemap groundTilemap;

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
    }

    public void Activate()
    {
        Debug.Log($"Instrument {gameObject.name} activated at position {transform.position}");
        Debug.Log($"Attack direction: {attackDirection}, Range: {attackRange}");

        // Play effect and sound
        if (activationEffect != null)
        {
            activationEffect.Play();
            Debug.Log("Playing activation effect");
        }
        if (instrumentSound != null)
        {
            instrumentSound.Play();
            Debug.Log("Playing instrument sound");
        }

        // Check for enemies in line
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            attackDirection,
            attackRange,
            enemyLayer
        );

        Debug.Log($"RaycastAll found {hits.Length} hits on enemy layer {enemyLayer.value}");

        foreach (var hit in hits)
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name} at distance {hit.distance}");
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Debug.Log($"Enemy found and taking damage");
                enemy.TakeDamage();
            }
        }

        // Debug visualization - make ray more visible
        Debug.DrawRay(transform.position, attackDirection * attackRange, Color.red, 1f, false);
        // Add a second ray in different color for better visibility
        Debug.DrawRay(transform.position, attackDirection * attackRange, Color.yellow, 0.5f, false);

        // Draw attack range circle
        DebugDrawCircle(transform.position, 0.5f, Color.blue, 1f);
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