using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentObjectPool<T> where T : Component
{
    public event Action<T> OnObjectCreated;

    public int poolMaxSize;
    public int poolAddSize;
    protected readonly List<T> pooledObjects = new();

    private int currentlyUsedObjects = 0;
    private int currentPoolSize = 0;
    private readonly Transform caller;

    public ComponentObjectPool(Transform caller, int poolMaxSize = 6, int poolAddSize = 3, Action<T> ObjectCreatedCallback = null)
    {
        if (poolMaxSize < 1)
        {
            throw new Exception("Pool max size must be bigger than 0");
        }

        if (poolAddSize < 1)
        {
            throw new Exception("Pool add size must be bigger than 0");
        }

        this.poolMaxSize = poolMaxSize;
        this.poolAddSize = poolAddSize;
        this.caller = caller;
        OnObjectCreated = ObjectCreatedCallback;

        GeneratePool();
    }

    public T GetAvailablePoolObject()
    {
        if (currentlyUsedObjects == poolMaxSize)
        {
            return null;
        }

        if (currentlyUsedObjects == currentPoolSize)
        {
            GeneratePool();
        }

        foreach (T a in pooledObjects)
        {
            if (!a.gameObject.activeSelf)
            {
                ++currentlyUsedObjects;
                return a;
            }
        }

        return null;
    }

    public List<T> GetPoolObjects()
    {
        return pooledObjects;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        --currentlyUsedObjects;
    }

    private void GeneratePool()
    {
        int addSize = Mathf.Min(poolAddSize, poolMaxSize - currentPoolSize);

        for (int i = 0; i < addSize; ++i)
        {
            GameObject obj = new("PoolObject");
            obj.SetActive(false);
            obj.transform.parent = caller;
            obj.transform.localPosition = Vector3.zero;
            T newObj = obj.AddComponent<T>();
            OnObjectCreated?.Invoke(newObj);
            pooledObjects.Add(newObj);
        }

        currentPoolSize += addSize;
    }
}