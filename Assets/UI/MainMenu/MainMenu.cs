using Assets.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] public string[] GameObjectsFolderPath;
        [SerializeField] public string[] ExcludedFolders;
        public void PlayLastGame()
        {
            LoadGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void LoadGame()
        {
            ObjectPoolHandler.SetupObjectPool(GameObjectsFolderPath, ExcludedFolders);
        }

        public void ExitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
