using Assets.Global.SceneHandlers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    #region Unity life cycle

    protected override void Awake()
    {
        base.Awake();
        OnInputBlockedStateChanged += (bool isBlocked) => IsInputBlocked = isBlocked;

        async UniTaskVoid ToggleCommandLine() => await WindowHandler.Instance.SwitchWindow("CommandLine", isTab: false);
        async UniTaskVoid OpenPlayerInventory() => await WindowHandler.Instance.SwitchWindow("PlayerInventory", isTab: true);

        _keyCommands = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Slash, () => { ToggleCommandLine().Forget(); } },
            { KeyCode.I, () => { OpenPlayerInventory().Forget(); } }
        };
    }

    private void Update()
    {
        if (!IsInputBlocked)
            foreach (var kpv in _keyCommands)
                if (Input.GetKeyDown(kpv.Key)) kpv.Value?.Invoke();
    }

    #endregion
}