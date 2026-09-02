using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenu : UIWindow
    {
        private readonly List<GameSessionData> _saves = new();
        private bool _isLoadingGame;

        #region Setup

        private void Awake()
        {
            var paths = FilesPathExtractor.GetFilePaths(new string[] { Path.Combine(Application.streamingAssetsPath, "Saves") });
            foreach (var path in paths) _saves.Add(DataFileHandler.LoadFromJson<GameSessionData>(path));
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            SetUIInputState(true);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            SetUIInputState(false);
        }

        private void SetUIInputState(bool isMenuOpen)
        {
            Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMenuOpen;
        }

        #endregion

        #region UI API

        public async void PlayGameFromUI(int index) => await PlayGame(index);

        public async Task LoadGameFromUI(int index) => await LoadGame(index);

        public void ExitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        #endregion

        #region UI Services

        private GameSessionData LoadGameData(int index) => _saves[index];

        private async Task PlayGame(int index)
        {
            if (_isLoadingGame) return;
            _isLoadingGame = true;
            try
            {
                var save = LoadGameData(index);
                GameSessionHandler.Data = save;

                await SceneController.Instance.NextLocation(save.locationName);
                await GameSessionHandler.Instance.SpawnPlayer();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке игры: {ex.Message}");
            }
            finally
            {
                _isLoadingGame = false;
            }
        }

        private async Task LoadGame(int index)
        {

        }

        #endregion
    }
}