using System;
using System.Collections.Generic;
using UI.GUI.CommandLine;
using UnityEngine;

public class GUIHandler : SingletonMonoBehaviour<GUIHandler>
{
    [SerializeField] private CommandLine _commandLine;
    private Dictionary<KeyCode, Action> _keyCommands;

    #region InputBlock

    public static event Action<bool> OnInputBlockedStateChanged;
    public bool IsInputBlocked { get; private set; }

    public void SetInputBlocked(bool isBlocked)
    {
        IsInputBlocked = isBlocked;
        OnInputBlockedStateChanged?.Invoke(isBlocked);
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        OnInputBlockedStateChanged += (bool isBlocked) => IsInputBlocked = isBlocked;

        // Используем лямбда-выражение со сэйфти-проверкой на null
        _keyCommands = new Dictionary<KeyCode, Action>
        {
            {
                KeyCode.Slash, () =>
                {
                    if (_commandLine != null)
                    {
                        _commandLine.Switch();
                    }
                    else
                    {
                        Debug.LogError("[GUIHandler] Ссылка на CommandLine не задана в Inspector!", this);
                    }
                }
            }
        };
    }

    private void Update()
    {
        if (!IsInputBlocked) GetInput();
    }

    private void GetInput()
    {
        foreach (var kpv in _keyCommands)
            if (Input.GetKeyDown(kpv.Key)) kpv.Value?.Invoke();
    }

    public void HandleTabToggled(bool isConsoleOpen)
    {

    }
}