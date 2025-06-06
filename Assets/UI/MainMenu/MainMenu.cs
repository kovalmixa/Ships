using System.IO;
using Assets.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayLastGame()
        {
            LoadGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void LoadGame()
        {
            //code for loading data from save files
        }

        public void ExitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
