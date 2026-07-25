using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        [SerializeField] private GameObject[] _dontDestroyOnLoadObj;

        protected override void Awake()
        {
            base.Awake();
            Setup();
        }

        private void Setup()
        {
            DontDestroyOnLoad(gameObject);
            foreach (var obj in _dontDestroyOnLoadObj) DontDestroyOnLoad(obj);
        }

        public void NextLocation(string locationName)
        {
            if (Application.CanStreamedLevelBeLoaded(locationName))
            {
                ObjectPoolHandler.RealeasePools();
                SceneManager.LoadScene(locationName, LoadSceneMode.Single);
                //Добавить загрузочный экран
            }
            else Debug.LogWarning($"Scene not found by name {locationName}");
        }

        public static GameObject GetNodeByName(string name)
        {
            GameObject node = GameObject.Find(name);
            if (node == null)
            {
                node = GameObject.Find("DontDestroyOnLoad").transform.Find(name).gameObject;
                if (node == null) return null;
            }
            return node;
        }

        public static T GetNodeByType<T>() where T : Component
        {
            T node = UnityEngine.Object.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null) node = dontDestroy.GetComponentInChildren<T>(true);
            }

            return node;
        }

        public static ObjectPoolHandler GetPoolHandler(string poolName)
        {
            ObjectPoolHandler poolHandler;
            try
            {
                var objectPool = GetNodeByName("ObjectPools");
                if (objectPool == null) throw new Exception("Master pool node not found");
                var specifiedPool = objectPool.transform.Find(poolName).gameObject;
                if (specifiedPool == null) throw new Exception($"Pool: {poolName} node not found");
                poolHandler = specifiedPool.GetComponent<ObjectPoolHandler>();
                if (poolHandler == null) throw new Exception("PoolHandler component not found");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            return poolHandler;
        }

    }
}
