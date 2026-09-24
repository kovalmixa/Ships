using System;
using System.Collections.Generic;
using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GUIHandler : SingletonMonoBehaviour<GUIHandler>
{
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

        async UniTaskVoid ToggleCommandLine() => await WindowHandler.Instance.SwitchWindow("CommandLine", isTab: false);

        _keyCommands = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Slash, () => { ToggleCommandLine().Forget(); } }
        };
    }

    private void Update()
    {
        if (!IsInputBlocked) GetInput();
    }

    private void GetInput()
    {
        foreach (var kpv in _keyCommands) if (Input.GetKeyDown(kpv.Key)) kpv.Value?.Invoke();
    }

    public void HandleTabToggled(bool isConsoleOpen)
    {

    }
}