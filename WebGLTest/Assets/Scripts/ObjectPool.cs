using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int initialSize;
        public List<GameObject> pool;
    }

    public List<PoolItem> pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        poolDict = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var poolItem in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < poolItem.initialSize; i++)
            {
                GameObject obj = Instantiate(poolItem.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDict.Add(poolItem.prefab, objectPool);
        }
    }

    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (poolDict.TryGetValue(prefab, out Queue<GameObject> objectPool))
        {
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(prefab, position, rotation);
                return obj;
            }
        }
        else
        {
            Debug.LogWarning("Prefab not found in pool dictionary: " + prefab.name);
            GameObject obj = Instantiate(prefab, position, rotation);
            return obj;
        }
    }

    public void ReleaseObject(GameObject obj, float delay = 0f)
    {
        if (delay > 0)
        {
            StartCoroutine(ReleaseAfterDelay(obj, delay));
        }
        else
        {
            DoReleaseObject(obj);
        }
    }

    private IEnumerator ReleaseAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        DoReleaseObject(obj);
    }

    private void DoReleaseObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            GameObject prefab = GetPrefab(obj);
            if (prefab != null && poolDict.TryGetValue(prefab, out Queue<GameObject> objectPool))
            {
                objectPool.Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    private GameObject GetPrefab(GameObject obj)
    {
        foreach (var poolItem in pools)
        {
            if (poolItem.pool.Contains(obj))
            {
                return poolItem.prefab;
            }
        }
        return null;
    }
}
