using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        private List<Transform> Pools = new();
        public GameObject[] DontDestroyOnLoadObj;

        private void Awake()
        {
            base.Awake();
            Setup();
        }

        private void Setup()
        {
            DontDestroyOnLoad(gameObject);
            foreach (var obj in DontDestroyOnLoadObj) DontDestroyOnLoad(obj);
            foreach (Transform child in SceneNodesHandler.GetNode("ObjectPools").transform) Pools.Add(child);
        }

        public void NextLocation(string locationName)
        {
            if (Application.CanStreamedLevelBeLoaded(locationName))
            {
                ClearPools();
                SceneManager.LoadScene(locationName, LoadSceneMode.Single);
                //Добавить загрузочный экран
            }
            else Debug.LogWarning($"Scene not found by name {locationName}");
        }

        private void ClearPools()
        {
            foreach (var pool in Pools)
            {
                foreach (Transform child in pool)
                {
                    ObjectPoolHandler poolHandler = pool.GetComponent<ObjectPoolHandler>();
                    if (child.gameObject.activeSelf) poolHandler.Return(child.gameObject);
                }
            }
        }
    }
}
