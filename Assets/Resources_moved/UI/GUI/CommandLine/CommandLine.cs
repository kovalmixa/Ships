using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.GUI.CommandLine
{
    public class CommandLine : UIWindow
    {
        [SerializeField] private TMP_InputField _inputField;

        public struct CLLMessage
        {
            public string text;
            public Color color;

            public CLLMessage(string text, Color color)
            {
                this.text = text;
                this.color = color;
            }
        }

        private readonly List<CLLMessage> _messageLog = new();
        private readonly List<TextMeshProUGUI> _textBoxBuffer = new();
        private Dictionary<string, Action> _commandMap;

        public event Action<bool> OnToggled;
        public bool IsOpen => gameObject.activeSelf;

        #region Unity Lifecycle

        private void Awake()
        {
            _commandMap = new Dictionary<string, Action>
            {
                { "print", () => WriteMessage("HelloWorld") }
            };
        }

        private void OnEnable()
        {
            DebugHandler.OnLog += HandleLog;
            if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(true);
            FocusInputFieldAsync().Forget();
            OnToggled?.Invoke(true);
        }

        private async UniTaskVoid FocusInputFieldAsync()
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            if (_inputField != null)
            {
                _inputField.text = string.Empty;
                _inputField.Select();
                _inputField.ActivateInputField();
            }
        }

        private void OnDisable()
        {
            DebugHandler.OnLog -= HandleLog;

            if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(false);
            if (_inputField != null) _inputField.DeactivateInputField();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

            OnToggled?.Invoke(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseConsole();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) ExecuteCommand();
        }

        #endregion

        #region Public API

        public void Switch() => gameObject.SetActive(!gameObject.activeSelf);

        public void CloseConsole() => CloseAsync().Forget();

        #endregion

        #region Command Processing

        private void ExecuteCommand()
        {
            if (_inputField == null || string.IsNullOrWhiteSpace(_inputField.text)) return;

            string commandText = _inputField.text.Trim().ToLower();

            if (_commandMap.TryGetValue(commandText, out Action command)) command?.Invoke();
            else WriteMessage($"Unknown command: '{commandText}'", Color.red);

            _inputField.text = string.Empty;
            _inputField.ActivateInputField();
        }

        private void HandleLog(string message)
        {
            WriteMessage(message, Color.green);
        }

        private void WriteMessage(string message, Color color = default)
        {
            if (color == default) color = Color.white;
            _messageLog.Add(new CLLMessage(message, color));
            Debug.Log(message);
        }

        #endregion
    }
}