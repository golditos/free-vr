using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnHeightMode
    {
        UseSpawnPointHeight,
        Grounded,
        RandomHeightRange,
        FixedHeight
    }

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform targetOverride;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAlive = 5;
    [SerializeField] private float randomRadius = 0f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool autoStart = true;

    [Header("Height Settings")]
    [SerializeField] private SpawnHeightMode heightMode = SpawnHeightMode.UseSpawnPointHeight;
    [SerializeField] private float fixedHeight = 0f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float groundOffset = 0.05f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private float groundRaycastStartHeight = 20f;
    [SerializeField] private float groundRaycastDistance = 50f;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private Coroutine spawnRoutine;

    private void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawnRoutine != null) return;

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine == null) return;

        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        if (spawnOnStart)
        {
            TrySpawnEnemy();
        }

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            TrySpawnEnemy();
        }
    }

    public void TrySpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{name}: No tiene Enemy Prefab asignado.");
            return;
        }

        CleanAliveList();

        if (aliveEnemies.Count >= maxAlive)
        {
            return;
        }

        Transform selectedSpawnPoint = GetRandomSpawnPoint();

        Vector3 spawnPosition = GetSpawnPosition(selectedSpawnPoint);
        Quaternion spawnRotation = GetSpawnRotation(selectedSpawnPoint);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        aliveEnemies.Add(enemy);

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            if (targetOverride != null)
            {
                enemyAI.SetTarget(targetOverride);
            }

            enemyAI.SetFlyingHeight(spawnPosition.y);
        }
    }

    private Vector3 GetSpawnPosition(Transform selectedSpawnPoint)
    {
        Vector3 position = selectedSpawnPoint != null ? selectedSpawnPoint.position : transform.position;

        if (randomRadius > 0f)
        {
            Vector2 randomCircle = Random.insideUnitCircle * randomRadius;
            position += new Vector3(randomCircle.x, 0f, randomCircle.y);
        }

        position.y = GetSpawnHeight(position);

        return position;
    }

    private float GetSpawnHeight(Vector3 position)
    {
        switch (heightMode)
        {
            case SpawnHeightMode.Grounded:
                return GetGroundHeight(position);

            case SpawnHeightMode.RandomHeightRange:
                return Random.Range(minHeight, maxHeight);

            case SpawnHeightMode.FixedHeight:
                return fixedHeight;

            case SpawnHeightMode.UseSpawnPointHeight:
            default:
                return position.y;
        }
    }

    private float GetGroundHeight(Vector3 position)
    {
        Vector3 rayStart = new Vector3(
            position.x,
            position.y + groundRaycastStartHeight,
            position.z
        );

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundRaycastDistance, groundLayer))
        {
            return hit.point.y + groundOffset;
        }

        Debug.LogWarning($"{name}: No se ha encontrado suelo debajo del punto de spawn. Usando altura actual.");
        return position.y;
    }

    private Quaternion GetSpawnRotation(Transform selectedSpawnPoint)
    {
        if (selectedSpawnPoint != null)
        {
            return selectedSpawnPoint.rotation;
        }

        return transform.rotation;
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private void CleanAliveList()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (randomRadius > 0f)
        {
            Gizmos.DrawWireSphere(transform.position, randomRadius);
        }

        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;

            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);
                }
            }
        }

        Gizmos.color = Color.cyan;

        if (heightMode == SpawnHeightMode.RandomHeightRange)
        {
            Vector3 minPos = transform.position;
            minPos.y = minHeight;

            Vector3 maxPos = transform.position;
            maxPos.y = maxHeight;

            Gizmos.DrawWireSphere(minPos, 0.4f);
            Gizmos.DrawWireSphere(maxPos, 0.4f);
            Gizmos.DrawLine(minPos, maxPos);
        }
    }
}