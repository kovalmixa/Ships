using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        [SerializeField] private GameObject[] _dontDestroyOnLoadObj;
        public static event Action OnBeforeSceneLoad;

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
                OnBeforeSceneLoad?.Invoke();
                SceneManager.LoadScene(locationName, LoadSceneMode.Single);
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
            T node = FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null) node = dontDestroy.GetComponentInChildren<T>(true);
            }

            return node;
        }
    }
}
