using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawnerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private List<Transform> usedPositions;
    [SerializeField] private int minSpawnItems = 0, maxSpawnItems = 3;
    [SerializeField] private List<GameObject> spawnedItems;

    private void OnEnable()
    {
        SpawnRandomItem();
    }

    public void SpawnRandomItem()
    {
        for (int i = spawnPositions.Count - 1; i >= 0 ; i--)
        {
            if (spawnPositions[i].childCount > 0)
            {
                var child = spawnPositions[i].GetChild(0);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        var amountToSpawn = Random.Range(minSpawnItems, maxSpawnItems + 1);
        for (int i = 0; i < amountToSpawn; i++)
        {
            var itemToSpawn = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
            var randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            while (usedPositions.Contains(randomPos) && usedPositions.Count < spawnPositions.Count)
            {
                randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            }
            if (itemToSpawn != null && randomPos != null)
            {
                while (usedPositions.Contains(randomPos) && usedPositions.Count < spawnPositions.Count)
                {
                    randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
                    if (!usedPositions.Contains(randomPos))
                    {
                        usedPositions.Add(randomPos);
                    }
                }
                var itemSpawned = Instantiate(itemToSpawn, randomPos);
                spawnedItems.Add(itemSpawned);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapSpawnerController.SpawnMap(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapSpawnerController.DespawnMap(gameObject);
        }
    }
}
