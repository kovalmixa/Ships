using System;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public static class SceneNodesHandler
    {
        public static GameObject GetNode(string name)
        {
            GameObject node = GameObject.Find(name);
            if (node == null)
            {
                node = GameObject.Find("DontDestroyOnLoad").transform.Find(name).gameObject;
                if (node == null) return null;
            }
            return node;
        }

        public static ObjectPoolHandler GetPoolHandler(string poolName)
        {
            ObjectPoolHandler poolHandler;
            try
            {
                var objectPool = SceneNodesHandler.GetNode("ObjectPools");
                if (objectPool == null) throw new Exception("Master pool node not found");
                var specifiedPool = objectPool.transform.Find(poolName).gameObject;
                if (specifiedPool == null) throw new Exception($"Pool: {poolName} node not found");
                poolHandler = specifiedPool.GetComponent<ObjectPoolHandler>();
                if (poolHandler == null) throw new Exception("PoolHandler component not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return poolHandler;
        }
    }
}
