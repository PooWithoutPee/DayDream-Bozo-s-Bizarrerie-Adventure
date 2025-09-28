using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject meleeEnemyPrefab; // Enemy that approaches player
    public GameObject rangedEnemyPrefab; // Enemy that shoots projectiles
    
    [Header("Spawning Settings")]
    public Transform player; // Player transform
    public int maxEnemies = 10; // Maximum enemies at once
    public float spawnRate = 2f; // Seconds between spawns
    public float spawnDistance = 20f; // How far from player to spawn
    
    [Header("Enemy Type Chances")]
    [Range(0f, 1f)]
    public float meleeSpawnChance = 0.6f; // 60% chance for melee, 40% for ranged

    [Header("Spawn Area")]
    public float spawnRangeX = 15f; // Spawn area width
    public float spawnRangeZ = 15f; // Spawn area depth
    public float spawnHeight = 4f; // Height above ground to spawn enemies
    
    // Keep track of spawned enemies
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float nextSpawnTime = 0f;
    
    void Update()
    {
        // Clean up destroyed enemies from list
        CleanupEnemyList();
        
        // Spawn new enemy if conditions are met
        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
            SpawnRandomEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
    
    void SpawnRandomEnemy()
    {
        // Choose enemy type randomly
        GameObject enemyToSpawn = (Random.value < meleeSpawnChance) ? meleeEnemyPrefab : rangedEnemyPrefab;
        
        // Get random spawn position around player
        Vector3 spawnPos = GetRandomSpawnPosition();
        
        // Create enemy
        GameObject enemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        
        // Add to our tracking list
        spawnedEnemies.Add(enemy);
        
        string enemyType = (enemyToSpawn == meleeEnemyPrefab) ? "Melee" : "Ranged";
        Debug.Log($"Spawned {enemyType} enemy at {spawnPos}. Total enemies: {spawnedEnemies.Count}");
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPos = player.position;
        Vector3 spawnPos;
        
        // Keep trying until we get a position far enough from player
        do
        {
            // Random position within spawn range
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);
            
            spawnPos = playerPos + new Vector3(randomX, 0f, randomZ);
        }
        while (Vector3.Distance(spawnPos, playerPos) < spawnDistance);
        
        spawnPos.y = spawnHeight;
        return spawnPos;
    }
    
    void CleanupEnemyList()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }
    
    public int GetEnemyCount()
    {
        CleanupEnemyList();
        return spawnedEnemies.Count;
    }
    
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }
}