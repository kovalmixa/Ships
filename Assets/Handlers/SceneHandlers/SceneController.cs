using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : MonoBehaviour
    {
        private static SceneController _instance;
        private List<Transform> Pools = new();
        public GameObject[] DontDestroyOnLoadObj;
        private void Awake()
        {
            if (_instance == null) Setup();
            else Destroy(gameObject);
        }

        private void Setup()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var obj in DontDestroyOnLoadObj) DontDestroyOnLoad(obj);
            foreach (Transform child in SceneNodesHandler.GetNode("ObjectPools").transform) Pools.Add(child);
        }

        public void NextLocation(string locationName)
        {
            if (Application.CanStreamedLevelBeLoaded(locationName))
            {
                ClearPools();
                SceneManager.LoadSceneAsync(locationName);
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
