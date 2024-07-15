using System;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolUtility : MonoBehaviour
{
    private enum PoolType
    {
        Stack,
        LinkedList,
    }

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (_objectPool != null) return _objectPool;

            _objectPool = _poolType switch
            {
                PoolType.Stack => new ObjectPool<GameObject>(CreateObject, OnObjectGet, OnObjectRelease,
                    OnObjectDestroy, _collectionChecks, _defaultPoolCapacity, _maxPoolCapacity),
                PoolType.LinkedList => new LinkedPool<GameObject>(CreateObject, OnObjectGet, OnObjectRelease,
                    OnObjectDestroy, _collectionChecks, _maxPoolCapacity),
                _ => throw new ArgumentOutOfRangeException()
            };

            return _objectPool;
        }
    }
    
    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private PoolType _poolType;
    [SerializeField] private bool _collectionChecks = true;
    [SerializeField] private int _defaultPoolCapacity = 10;
    [SerializeField] private int _maxPoolCapacity = 20;
    
    IObjectPool<GameObject> _objectPool;
    
    private GameObject CreateObject()
    {
        if (_objectPrefab == null) return null;
        if (_parent == null) return null;
        
        return Instantiate(_objectPrefab, _parent);
    }
    
    private static void OnObjectGet(GameObject poolObject)
    {
        poolObject.SetActive(true);
    }
    
    private static void OnObjectRelease(GameObject poolObject)
    {
        poolObject.SetActive(false);
    }
    
    private static void OnObjectDestroy(GameObject poolObject)
    {
        Destroy(poolObject);
    }
}