using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public float minSpawnInterval, maxSpawnInterval;
    public float minSpawnDistance, maxSpawnDistance;
    public int maxSpawnCount;
    public GameObject enemyPrefab;

    public Transform enemyRoot;

    public PlayerInfoScriptableObject playerInfo;

    private HashSet<GameObject> spawnedEnemies = new HashSet<GameObject>();
    
    private const int SPAWN_CAP = 45;

    void Start()
    {
        StartCoroutine(CheckForDestroy());
        StartCoroutine(SpawnEnemyRoutine());
    }

    void SpawnEnemies()
    {
        int count = Random.Range(0, maxSpawnCount) + 1;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = playerInfo.position + Random.onUnitSphere * Random.Range(minSpawnDistance, maxSpawnDistance);
            SpawnEnemy(pos);
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        if (spawnedEnemies.Count >= SPAWN_CAP) return;
        
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemyRoot);
        spawnedEnemies.Add(enemy);
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private const float DESTROY_CHECK_INTERVAL = 1, DESTROY_CHECK_DISTANCE = 240;

    IEnumerator CheckForDestroy()
    {
        while (true)
        {
            yield return new WaitForSeconds(DESTROY_CHECK_INTERVAL);
            foreach (GameObject enemy in new List<GameObject>(spawnedEnemies))
            {
                if (Vector3.SqrMagnitude(enemy.transform.position - playerInfo.position) > DESTROY_CHECK_DISTANCE * DESTROY_CHECK_DISTANCE)
                {
                    Destroy(enemy);
                    spawnedEnemies.Remove(enemy);
                }
            }

        }
    }

}
