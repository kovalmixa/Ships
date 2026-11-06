using Assets.Global.SceneHandlers.GameSession;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
    public class MainMenu : UIWindow
    {
        private bool _isLoadingGame;

        private Button _continueButton;
        private Button _saveLoadButton;
        private Button _newGameButton;
        private Button _optionsButton;
        private Button _quitButton;

        #region Setup

        private void OnEnable()
        {
            if (RootVisualElement == null) return;
            GUIHandler.Instance.SetInputBlocked(true);

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
            if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(false);
        }

        #endregion

        #region Button Global

        private void OnContinueClicked() => LaunchGameSession(0);

        private void OnSaveLoadClicked()
        {
            Debug.Log("Открыть окно сохранения/загрузки");
        }

        private void OnNewGameClicked() => LaunchGameSession(0);

        private void OnOptionsClicked()
        {
            Debug.Log("Открыть настройки");
        }

        #endregion

        #region UI Services

        private void LaunchGameSession(int saveIndex)
        {
            if (_isLoadingGame) return;
            _isLoadingGame = true;

            try
            {
                GameSessionController.Instance.StartGameSession(saveIndex);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при запуске сессии из UI: {ex.Message}");
                _isLoadingGame = false;
            }
        }

        public void ExitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        #endregion
    }
}