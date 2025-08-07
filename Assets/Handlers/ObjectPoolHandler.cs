using System.Collections.Generic;
using UnityEngine;

namespace Assets.Handlers
{
    public class ObjectPoolHandler : MonoBehaviour
    {
        public GameObject prefab;
        public int initialSize = 100;
        private Queue<GameObject> pool = new();

        void Start()
        {
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, this.transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            if (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            return null;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
