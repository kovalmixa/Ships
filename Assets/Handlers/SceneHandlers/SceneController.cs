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
    }
}
