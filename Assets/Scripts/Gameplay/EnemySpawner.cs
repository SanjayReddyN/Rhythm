using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int spawnEveryNBeats = 4;
    [SerializeField] private Tilemap groundTilemap;

    private int beatCounter = 0;
    private bool isSpawning = false;
    private void Start()
    {
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();
        }

        if (FMODManager.Instance != null)
        {
            FMODManager.Instance.OnBeat += HandleBeat;
        }
    }

    private void HandleBeat()
    {
        beatCounter++;
        if (beatCounter >= spawnEveryNBeats)
        {
            SpawnEnemy();
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