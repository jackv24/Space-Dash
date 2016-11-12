using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ObjectPooler
{
    class ObjectPool
    {
        public ObjectPool(string tag, GameObject prefab)
        {
            this.tag = tag;
            this.prefab = prefab;
        }

        //Identifies the group of objects that this is e.g, 'Bullets'
        public string tag = "GameObject";
        //The prefab to spawn if no objects are available
        public GameObject prefab;

        public List<GameObject> objects = new List<GameObject>();

        public GameObject GetObject()
        {
            //Attempt to reuse object
            foreach (var o in objects)
            {
                if (!o.activeSelf)
                {
                    //Re-enable and reset object
                    o.SetActive(true);
                    return o;
                }
            }

            //If object cannot be reused, create a new one
            GameObject obj = (GameObject)GameObject.Instantiate(prefab);
            objects.Add(obj);

            //Parent to pooler object
            GameObject poolerParent = GameObject.Find("PooledObjects");
            if (!poolerParent)
                poolerParent = new GameObject("PooledObjects");
            obj.transform.SetParent(poolerParent.transform);

            return obj;
        }
    }

    //All object pools
    private static List<ObjectPool> pools = new List<ObjectPool>();

    public static GameObject Spawn(string tag, GameObject prefab)
    {
        //Find existing pool
        foreach (var pool in pools)
        {
            //If pool is found, get object from that pool
            if (pool.tag == tag)
            {
                return pool.GetObject();
            }
        }

        //If no pool is found, create a new pool
        ObjectPool newPool = new ObjectPool(tag, prefab);
        pools.Add(newPool);
        //Get object from this new pool
        return newPool.GetObject();
    }

    public static void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    public static void PurgePools()
    {
        while (pools.Count > 0)
        {
            while (pools[0].objects.Count > 0)
            {
                GameObject.Destroy(pools[0].objects[0]);
                pools[0].objects.RemoveAt(0);
            }

            pools.RemoveAt(0);
        }
    }
}
