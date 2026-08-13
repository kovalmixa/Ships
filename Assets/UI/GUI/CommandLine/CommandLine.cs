using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.GUI.CommandLine
{
    public class CommandLine : MonoBehaviour
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
        public bool IsOpen { get; private set; }

        public void Switch()
        {
            IsOpen = !IsOpen;
            gameObject.SetActive(IsOpen);
            OnToggled?.Invoke(IsOpen);
            GUIHandler.Instance.SetInputBlocked(IsOpen);
        }

        private void Awake()
        {
            DebugHandler.OnLog += (string message) => WriteMessage(message, Color.green);

            _commandMap = new()
        {
            { "print", () => WriteMessage("HelloWorld") }
        };
        }

        private void OnEnable()
        {
            _inputField.text = "";
            _inputField.Select();
            _inputField.ActivateInputField();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Switch();
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) ExecuteCommand();
        }

        private void ExecuteCommand()
        {
        }

        private void WriteMessage(string message, Color color = default)
        {
            if (color == default) color = Color.white;
            _messageLog.Add(new CLLMessage(message, color));
            Debug.Log(message);
        }
    }
}

