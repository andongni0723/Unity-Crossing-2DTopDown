using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;


public enum SpawnPositionType
{
    Up, Down, Left, Right
}

[Serializable]
public class SpawnPosition
{
    public string name; // Inspector Window
    public SpawnPositionType type;
    public Vector2 minSpawnRange;
    public Vector2 maxSpawnRange; 
    public Quaternion spawnRotation;
}

[Serializable]
public class EnemySpawn
{
    public string name;
    public List<GameObject> kindOfEnemyList = new List<GameObject>();
    public bool isRandomPositionSpawn = true;
    public int waveMaxSpawnCount = 10;
    public int randomSpawnProbability;
    [HideInInspector]public int randomSpawnMin;
    [HideInInspector]public int randomSpawnMax;
}

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Settings")] 
    public bool isTest = false;
    public List<EnemySpawn> enemySpawnList = new List<EnemySpawn>();
    private Dictionary<string, int> KindOfEnemySpawnCount = new Dictionary<string, int>();
    public float waitNextSpawnTime = 1.5f;
    public float waitNextWaveTime = 3;
    
    [Space(15)]
    public List<SpawnPosition> SpawnPositionsList = new List<SpawnPosition>();
 
    private void Awake()
    {
        if (isTest)
        {
            Debug.LogWarning("Test Mode : Not Spawn Enemy");
            return;
        }

        SetEnemyRandomSpawnRange();
        StartCoroutine(SpawnEnemy());
    }

    void SetEnemyRandomSpawnRange()
    {
        int currentProbabilityNum = 0;
        foreach (var enemy in enemySpawnList)
        {
            enemy.randomSpawnMin = currentProbabilityNum;
            enemy.randomSpawnMax = currentProbabilityNum + enemy.randomSpawnProbability;
            currentProbabilityNum += enemy.randomSpawnProbability;

            Debug.Log($"{enemy.randomSpawnMin}, {enemy.randomSpawnMax}");
        }

        if (currentProbabilityNum != 100)
        {
            Debug.LogError("The Enemy Probability not not equal to 100%");
        }
    }

    IEnumerator SpawnEnemy()
    {
        foreach (var kindEnemy in enemySpawnList)
        {
            KindOfEnemySpawnCount.Add(kindEnemy.name, kindEnemy.waveMaxSpawnCount);
        }
        
        while (true)
        {
            // 1. Choose the Spawn Position
            // 2. Choose the kindEnemyList
            // 3. Choose the Enemy
            
            // Spawn Position
            int spawnCount = Random.Range(1, 5);
            int spawnPositionIndex = Random.Range(0, SpawnPositionsList.Count);
            SpawnPosition currentSpawnPosition = SpawnPositionsList[spawnPositionIndex];
            SpawnPositionType currentSpawnType = currentSpawnPosition.type;
            //Debug.Log(currentSpawnPosition.name);
            
            // Reset Spawn Count
            foreach (var kindEnemy in enemySpawnList)
            {
                KindOfEnemySpawnCount[kindEnemy.name] = kindEnemy.waveMaxSpawnCount;
            }
            
            // Spawn Enemy
            for (int i = 0; i < spawnCount; i++)
            {
                foreach (var kindEnemy in enemySpawnList)
                {
                    int spawnRangeNum = Random.Range(0, 100);
                    Debug.Log(spawnRangeNum);

                    if (spawnRangeNum >= kindEnemy.randomSpawnMin && spawnRangeNum <= kindEnemy.randomSpawnMax)
                    {
                        // the enemy spawn count to max in this wave
                        if(KindOfEnemySpawnCount[kindEnemy.name] <= 0) 
                        {
                            Debug.LogWarning("no");
                            continue;
                            
                        } 
                        
                        
                        // Enemy Spawn Event
                        switch (kindEnemy.name)
                        {
                            case "dangerousWall":
                                EventHandler.CallDangerousWallSpawn(currentSpawnType);
                                yield return new WaitForSeconds(3);
                                break;
                        }
                        
                        
                        // Spawn Enemy
                        int spawnEnemyIndex = Random.Range(0, kindEnemy.kindOfEnemyList.Count);
                        Vector3 spawnPosition = RandomSpawnPosition(
                            kindEnemy.isRandomPositionSpawn,
                            currentSpawnPosition.minSpawnRange.x,
                            currentSpawnPosition.maxSpawnRange.x,
                            currentSpawnPosition.minSpawnRange.y,
                            currentSpawnPosition.maxSpawnRange.y);

                        Instantiate(kindEnemy.kindOfEnemyList[spawnEnemyIndex], spawnPosition,
                            currentSpawnPosition.spawnRotation);
                        
                        KindOfEnemySpawnCount[kindEnemy.name]--;
                        
                        
                        yield return new WaitForSeconds(waitNextSpawnTime);
                    }
                }
            } 
            yield return new WaitForSeconds(waitNextWaveTime);
        }
    }

    private Vector3 RandomSpawnPosition(bool isRandomSpawn, float minX, float maxX, float minY, float maxY)
    {
        if (isRandomSpawn)
            return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
        else
            return new Vector3((minX + maxX) / 2, (minY + maxY) / 2);
    }
}
