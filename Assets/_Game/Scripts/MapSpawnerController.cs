using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class MapSpawnerController : MonoBehaviour
{
    public static Action<GameObject> onMapSpawn;
    
    [SerializeField] private GameObject mapSpawnPrefab;
    [SerializeField] private float spawnDistance = 5f;
    [SerializeField] private IObjectPool<GameObject> mapSpawnPool;
    [SerializeField] private int maxSpawnPoolSize = 20;
    [SerializeField] private List<GameObject> spawnedMapObjects = new();

    public GameObject MapSpawnPrefab
    {
        get => mapSpawnPrefab;
        set => mapSpawnPrefab = value;
    }
    
    public float SpawnDistance
    {
        get => spawnDistance;
        set => spawnDistance = value;
    }

    public IObjectPool<GameObject> MapSpawnPool
    {
        get
        {
            if (mapSpawnPool == null)
            {
                mapSpawnPool = new ObjectPool<GameObject>(CreateMapSpawnItem, OnTakeFromPool, OnReturnedToPool,
                    OnDestroyPoolObject, true, maxSpawnPoolSize);
            }

            return mapSpawnPool;
        }
    }

    public int MaxSpawnPoolSize
    {
        get => maxSpawnPoolSize;
        set => maxSpawnPoolSize = value;
    }

    public List<GameObject> SpawnedMapObjects
    {
        get => spawnedMapObjects;
        set => spawnedMapObjects = value;
    }

    private void Start()
    {
        for (int i = 0; i < maxSpawnPoolSize; i++)
        {
            MapSpawnPool.Get();
        }
    }

    public void OnEnable()
    {
        onMapSpawn += OnMapSpawn;
    }

    public void OnDisable()
    {
        onMapSpawn -= OnMapSpawn;
    }

    private void OnDestroyPoolObject(GameObject obj)
    {
        spawnedMapObjects.Remove(obj);
        Destroy(obj);
    }

    private void OnReturnedToPool(GameObject obj)
    {
        obj.SetActive(false);
        spawnedMapObjects.Remove(obj);
    }

    private void OnTakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
        var lastMapObject = spawnedMapObjects.LastOrDefault();
        if (lastMapObject != null)
        {
            obj.transform.position = new Vector3(0, 0, lastMapObject.transform.position.z + spawnDistance);
        }
        else
        {
            obj.transform.position = Vector3.zero;
        }

        if (!spawnedMapObjects.Contains(obj))
        {
            spawnedMapObjects.Add(obj);
        }


    }

    private GameObject CreateMapSpawnItem()
    {
        var go = Instantiate(mapSpawnPrefab, transform);
        var count = spawnedMapObjects.Count();
        var lastMapObject = spawnedMapObjects.LastOrDefault();
        if (lastMapObject != null)
        {
            go.transform.position = new Vector3(0, 0, lastMapObject.transform.position.z + spawnDistance);
        }
        else
        {
            go.transform.position = Vector3.zero;
        }
        if (!spawnedMapObjects.Contains(go))
        {
            spawnedMapObjects.Add(go);
        }   
        go.name = $"Map Spawn {count}";
        return go;
    }
    
    private void OnMapSpawn(GameObject obj)
    {
        MapSpawnPool.Get();
    }
    
    public static void SpawnMap(GameObject obj)
    {
        onMapSpawn?.Invoke(obj);
    }
}
