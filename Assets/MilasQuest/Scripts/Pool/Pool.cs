using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.Pools
{
    public class Pool
    {
        private Stack<PoolObject> _pooledObjects;
        private PoolData _poolData;
        private GameObject _poolsRootGameObject;
        private GameObject _poolParent;

        private static Dictionary<PoolData, Pool> _allPools = new Dictionary<PoolData, Pool>();

        public static Pool CreatePool(PoolData poolData)
        {
            if (_allPools.ContainsKey(poolData))
            {
                Debug.LogError("Pool for " + poolData.prefab.name + " has already been created!");
                return _allPools[poolData];
            }

            _allPools.Add(poolData, new Pool(poolData));
            return _allPools[poolData];
        }

        public static Pool GetPool(PoolData poolData)
        {
            if (_allPools.ContainsKey(poolData))
                return _allPools[poolData];
            else
                return null;
        }

        public static void DestroyPool(PoolData poolData)
        {
            Pool p = GetPool(poolData);
            if (p == null)
                return;
            p.DestroyPool();
            _allPools.Remove(poolData);
        }

        public static void DestroyAllPools()
        {
            foreach (Pool pool in _allPools.Values)
            {
                pool.DestroyPool();
            }
            _allPools.Clear();
        }

        public GameObject Spawn(Transform parent, Action<GameObject> OnSpawned = null)
        {
            if (_pooledObjects.Count == 0)
            {
                for (int i = 0; i < _poolData.increaseAmount; i++)
                {
                    InstantiatePoolObject();
                }
            }

            GameObject gameObject = _pooledObjects.Pop().gameObject;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            return gameObject;
        }

        public void Despawn(PoolObject poolObject)
        {
            if (poolObject.Pool != this)
            {
                Debug.LogError("Can't despawn objects from other pools!");
                return;
            }

            _pooledObjects.Push(poolObject);
            poolObject.gameObject.SetActive(false);
            poolObject.transform.SetParent(_poolParent.transform, false);
        }

        private Pool(PoolData poolData)
        {
            if (_allPools.ContainsKey(poolData))
            {
                Debug.LogError("Pool for " + poolData.prefab.name + " has already been created!");
                return;
            }

            _poolsRootGameObject = GameObject.Find("_Pools_");
            if (_poolsRootGameObject == null)
            {
                _poolsRootGameObject = new GameObject("_Pools_");
                _poolsRootGameObject.transform.localPosition = Vector3.one * -1000;
            }

            _poolParent = new GameObject("_Pool_" + poolData.prefab.name);
            _poolParent.transform.SetParent(_poolsRootGameObject.transform, false);

            _poolData = poolData;
            _pooledObjects = new Stack<PoolObject>();

            for (int i = 0; i < _poolData.initAmount; i++)
            {
                InstantiatePoolObject();
            }
        }

        private void InstantiatePoolObject()
        {
            GameObject gameObject = GameObject.Instantiate(_poolData.prefab) as GameObject;
            gameObject.AddComponent<PoolObject>().SetPool(this);
            gameObject.name = _poolData.prefab.name;
            gameObject.transform.SetParent(_poolParent.transform, false);
            gameObject.SetActive(false);
            _pooledObjects.Push(gameObject.GetComponent<PoolObject>());
        }

        private void DestroyPool()
        {
            while (_pooledObjects.Count > 0)
            {
                GameObject.Destroy(_pooledObjects.Pop().gameObject);
            }
            _pooledObjects.Clear();
        }
    }
}

