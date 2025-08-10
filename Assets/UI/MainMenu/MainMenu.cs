using System.IO;
using Assets.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {

        public void PlayGame(int index)
        {
            string locationName = LoadGame(0);
            SceneManager.LoadSceneAsync("LoadingScene");
            SceneManager.LoadSceneAsync("MainScene"); // <-locationName
        }

        private string LoadGame(int index)
        {
            //code for loading data from save files
            return "";
        }

        public void ExitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
