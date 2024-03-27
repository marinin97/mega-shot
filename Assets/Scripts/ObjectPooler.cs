using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject ObjectToPool;
    public GameObject Parent;
    public int AmountToPool;
    public bool ShouldExpand;

    [HideInInspector]
    public List<GameObject> PooledObjects; 
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<ObjectPoolItem> itemsToPool;

    private Dictionary<string, List<GameObject>> pooledObjects;

    private void Awake()
    {
        if (SharedInstance != null) Destroy(gameObject);
        SharedInstance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        pooledObjects = new Dictionary<string, List<GameObject>>();

        foreach (ObjectPoolItem item in itemsToPool)
        {
            item.PooledObjects = new List<GameObject>(); 

            for (int i = 0; i < item.AmountToPool; i++)
            {
                GameObject obj = Instantiate(item.ObjectToPool);
                obj.transform.parent = item.Parent.transform;
                obj.SetActive(false);
                item.PooledObjects.Add(obj); 
            }

            pooledObjects.Add(item.ObjectToPool.tag, item.PooledObjects); 
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (pooledObjects.ContainsKey(tag))
        {
            List<GameObject> pool = pooledObjects[tag];

            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    return pool[i];
                }
            }

            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.ObjectToPool.tag == tag)
                {
                    if (item.ShouldExpand)
                    {
                        GameObject obj = Instantiate(item.ObjectToPool);
                        obj.SetActive(false);
                        item.PooledObjects.Add(obj);
                        return obj;
                    }
                }
            }
        }

        return null;
    }

    public GameObject GetPooledObject(int index)
    {
        if (index >= 0 && index < itemsToPool.Count)
        {
            return itemsToPool[index].PooledObjects[0];
        }

        return null;
    }

    public int GetSize()
    {
        return itemsToPool.Count;
    }

    public int GetTotalPoolSize()
    {
        return pooledObjects.Count;
    }

    public int GetItemPoolSize(string tag)
    {
        return pooledObjects.ContainsKey(tag) ? pooledObjects[tag].Count : 0;
    }
}
