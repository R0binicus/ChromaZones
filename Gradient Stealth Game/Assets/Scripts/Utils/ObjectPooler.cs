using System.Collections.Generic;
using UnityEngine;

// Helper class for object pooling
public static class ObjectPooler
{
    // Create object pool using a prefab and amount to pool
    public static List<T> CreateObjectPool<T>(uint amountToPool, GameObject objectToPool)
    {
        List<T> pooledObjects = new List<T>();

        for (uint i = 0; i < amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj.GetComponent<T>());
        }

        return pooledObjects;
    }

    // Create object pool using a prefab and amount to pool under a given parent
    public static List<T> CreateObjectPool<T>(uint amountToPool, GameObject objectToPool, Transform parent)
    {
        List<T> pooledObjects = new List<T>();

        for (uint i = 0; i < amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(objectToPool, parent);
            obj.SetActive(false);
            pooledObjects.Add(obj.GetComponent<T>());
        }

        return pooledObjects;
    }

    //Retrieve a pooled object if it is not active in the hierarchy
    public static MonoBehaviour GetPooledObject(List<MonoBehaviour> pooledObjects)
    {
        foreach (MonoBehaviour obj in pooledObjects)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
    }

    // Return all objects to pool
    public static void ReturnObjectsToPool(List<MonoBehaviour> pooledObjects)
    {
        foreach (MonoBehaviour obj in pooledObjects)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }
}
