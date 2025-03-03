using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 12f; // Spawn every 12 beats
    [SerializeField] private float beatDuration = 1f;
    [SerializeField] private Tilemap groundTilemap;

    private float nextSpawnTime;
    private bool isSpawning = false;

    private void Start()
    {
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();
        }

        nextSpawnTime = Time.time + spawnInterval * beatDuration;
        isSpawning = true;
        Debug.Log("EnemySpawner initialized at position: " + transform.position);
    }

    private void Update()
    {
        if (!isSpawning) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval * beatDuration;
        }
    }

    private void SpawnEnemy()
    {
        // Ensure spawn position aligns with grid
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position);
        Vector3 spawnPosition = groundTilemap.GetCellCenterWorld(gridPosition);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Set up enemy references using the new setter method
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.SetGroundTilemap(groundTilemap);
        }

        Debug.Log($"Enemy spawned at grid position: {gridPosition}");
    }

    public void StartSpawning()
    {
        isSpawning = true;
        Debug.Log("Enemy spawning started");
    }

    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("Enemy spawning stopped");
    }
}