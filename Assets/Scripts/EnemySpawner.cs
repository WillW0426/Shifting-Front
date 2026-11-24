using UnityEngine;

//Manages Spawning Enemies.
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider2D spawnArea;

    [Header("Wave Settings")]
    [SerializeField] private float spawnInterval = 60f;
    [SerializeField] private int startingEnemies = 3;
    [SerializeField] private int enemyIncrease = 2;

    [Header("Targeting")]
    [Range(0f, 1f)]
    [SerializeField] private float unitTargetChance = 0.5f;

    [Header("Wave Status")]
    [SerializeField] private int currentWave = 0;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        int enemyCount = startingEnemies + currentWave * enemyIncrease;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        currentWave++;
    }

    private void SpawnEnemy()
    {
        // Randomly select an enemy type.
        GameObject prefab = enemyPrefabs[
            Random.Range(0, enemyPrefabs.Length)
        ];

        // Randomly select a spawn position.
        Bounds bounds = spawnArea.bounds;

        Vector3 spawnPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            0f
        );

        // Spawn the enemy.
        GameObject enemy = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        // Randomly assign targeting behaviour.
        EnemyAI ai = enemy.GetComponentInChildren<EnemyAI>();

        if (ai != null)
        {
            EnemyTarget target = Random.value < unitTargetChance
                ? EnemyTarget.Unit
                : EnemyTarget.Structure;

            ai.SetTarget(target);
        }
    }

    public float GetTimeUntilNextWave()
    {
        return Mathf.Max(0f, spawnInterval - spawnTimer);
    }

    public int GetNextWave()
    {
        return currentWave + 1;
    }
}
