using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
    public class MainMenu : UIWindow
    {
        private readonly List<GameSessionData> _saves = new();
        private bool _isLoadingGame;

        private Button _continueButton;
        private Button _saveLoadButton;
        private Button _newGameButton;
        private Button _optionsButton;
        private Button _quitButton;

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            var paths = FilesPathExtractor.GetFilePaths(new string[] { Path.Combine(Application.streamingAssetsPath, "Saves") });
            foreach (var path in paths) _saves.Add(DataFileHandler.LoadFromJson<GameSessionData>(path));
        }

        private void OnEnable()
        {
            if (RootVisualElement == null) return;

            _continueButton = RootVisualElement.Q<Button>("ContinueButton");
            _saveLoadButton = RootVisualElement.Q<Button>("SaveLoadButton");
            _newGameButton = RootVisualElement.Q<Button>("NewGameButton");
            _optionsButton = RootVisualElement.Q<Button>("Options");
            _quitButton = RootVisualElement.Q<Button>("Quit");

            if (_continueButton != null) _continueButton.clicked += OnContinueClicked;
            if (_saveLoadButton != null) _saveLoadButton.clicked += OnSaveLoadClicked;
            if (_newGameButton != null) _newGameButton.clicked += OnNewGameClicked;
            if (_optionsButton != null) _optionsButton.clicked += OnOptionsClicked;
            if (_quitButton != null) _quitButton.clicked += ExitGame;
        }

        private void OnDisable()
        {
            if (_continueButton != null) _continueButton.clicked -= OnContinueClicked;
            if (_saveLoadButton != null) _saveLoadButton.clicked -= OnSaveLoadClicked;
            if (_newGameButton != null) _newGameButton.clicked -= OnNewGameClicked;
            if (_optionsButton != null) _optionsButton.clicked -= OnOptionsClicked;
            if (_quitButton != null) _quitButton.clicked -= ExitGame;
        }

        #endregion

        #region Button Handlers

        private void OnContinueClicked() => PlayGameFromUI(0);

        private void OnSaveLoadClicked()
        {
            Debug.Log("Открыть окно сохранения/загрузки");
        }

        private void OnNewGameClicked() => PlayGameFromUI(0);

        private void OnOptionsClicked()
        {
            Debug.Log("Открыть настройки");
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