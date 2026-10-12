using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.GUI.CommandLine
{
    public class CommandLine : UIWindow
    {
        [SerializeField] private string _inputFieldName = "CommandLineInput";
        [SerializeField] private string _containerName = "MessageContainer";

        private TextField _inputField;
        private ScrollView _messageContainer;

        private readonly List<string> _commandHistory = new();
        private int _historyIndex = -1;

        public event Action<bool> OnToggled;

        protected override void OnOpened()
        {
            base.OnOpened();

            if (RootVisualElement != null)
            {
                _inputField = RootVisualElement.Q<TextField>(_inputFieldName);
                _messageContainer = RootVisualElement.Q<ScrollView>(_containerName);

                _inputField?.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

                RebuildMessageHistory();
            }

            if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(true);
            FocusInputFieldAsync().Forget();
            OnToggled?.Invoke(true);
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            if (_inputField != null) _inputField.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(false);
            OnToggled?.Invoke(false);
        }

        private async UniTaskVoid FocusInputFieldAsync()
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            if (this == null || !IsOpen)
                return;

            if (_inputField == null || _inputField.panel == null || RootVisualElement == null)
            {
                Debug.LogWarning("[Console] Cannot focus: window or panel is unavailable.");
                return;
            }

            _inputField.value = string.Empty;
            _inputField.Focus();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                ExecuteCommand();
                evt.StopPropagation();
                evt.PreventDefault();
            }
            else if (evt.keyCode == KeyCode.UpArrow)
            {
                NavigateHistory(-1);
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.DownArrow)
            {
                NavigateHistory(1);
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.Escape || evt.keyCode == KeyCode.Slash)
            {
                CloseConsole();
                evt.StopPropagation();
            }
        }

        #region Command Processing & History

        private void ExecuteCommand()
        {
            if (_inputField == null || string.IsNullOrWhiteSpace(_inputField.value)) return;

            string text = _inputField.value.Trim();
            _commandHistory.Add(text);
            _historyIndex = _commandHistory.Count;

            DuplicateMessage(text);
            /*
            string commandText = text.ToLower();
            if (_commandMap.TryGetValue(commandText, out Action command)) command?.Invoke();
            else WriteMessage($"Unknown command: '{commandText}'", Color.red);
            */

            _inputField.value = string.Empty;
            _inputField.Focus();
        }

        private void DuplicateMessage(string messageText)
        {
            if (_messageContainer == null) return;

            var currentTime = DateTime.Now.ToString("HH:mm:ss");
            string labelText = $">{currentTime}: {messageText}";
            Label messageLabel = new Label(labelText);
            messageLabel.AddToClassList("message-item");

            messageLabel.style.color = Color.white;
            messageLabel.style.fontSize = 14;
            messageLabel.style.marginBottom = 4;

            _messageContainer.Add(messageLabel);

            _messageContainer.schedule.Execute(() =>
            {
                if (_messageContainer != null)
                    _messageContainer.ScrollTo(messageLabel);
            });
        }

        private void RebuildMessageHistory()
        {
            if (_messageContainer == null) return;

            _messageContainer.Clear();

            foreach (var messageText in _commandHistory)
            {
                Label messageLabel = new Label(messageText);
                messageLabel.AddToClassList("message-item");

                messageLabel.style.color = Color.white;
                messageLabel.style.fontSize = 14;
                messageLabel.style.marginBottom = 4;

                _messageContainer.Add(messageLabel);
            }

            if (_commandHistory.Count > 0)
            {
                _messageContainer.schedule.Execute(() =>
                {
                    if (_messageContainer != null && _messageContainer.childCount > 0)
                        _messageContainer.ScrollTo(_messageContainer[_messageContainer.childCount - 1]);
                });
            }
        }

        private void NavigateHistory(int direction)
        {
            if (_commandHistory.Count == 0) return;

            _historyIndex += direction;
            _historyIndex = Mathf.Clamp(_historyIndex, 0, _commandHistory.Count);

            if (_historyIndex < _commandHistory.Count)
            {
                _inputField.value = _commandHistory[_historyIndex];
                _inputField.selectIndex = _inputField.value.Length;
            }
            else _inputField.value = string.Empty;
        }

        #endregion

        #region Public API

        public void WriteMessage()
        {

        }

        public void Switch()
        {
            if (IsOpen) CloseConsole();
            else OpenAsync().Forget();
        }

        public void CloseConsole() => CloseAsync().Forget();

        #endregion
    }
}