using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawnerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private int minSpawnItems = 0, maxSpawnItems = 3;
    [SerializeField] private List<GameObject> spawnedItems;

    private void Start()
    {
        SpawnRandomItem();
    }

    public void SpawnRandomItem()
    {
        var amountToSpawn = Random.Range(minSpawnItems, maxSpawnItems + 1);
        for (int i = 0; i < amountToSpawn; i++)
        {
            var itemToSpawn = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
            var randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            if (itemToSpawn != null && randomPos != null)
            {
                var itemSpawned = Instantiate(itemToSpawn, randomPos);
                spawnedItems.Add(itemSpawned);
                spawnPositions.Remove(randomPos);
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
