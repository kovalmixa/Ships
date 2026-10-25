using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GUIHandler : SingletonMonoBehaviour<GUIHandler>
{
    private Dictionary<KeyCode, Action> _keyCommands;
    private List<VisualElement> SelectedUIElements = new();
    public bool IsOverUI => SelectedUIElements.Count > 0;

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

        _keyCommands = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Slash, () => { ToggleCommandLine().Forget(); } }
        };
    }

    private void Update()
    {
        if (!IsInputBlocked) 
            foreach (var kpv in _keyCommands) 
                if (Input.GetKeyDown(kpv.Key)) kpv.Value?.Invoke();
    }

    #endregion

    #region Public API

    public void SetOnUI(VisualElement ve) => SelectedUIElements.Add(ve);
    public void UnSetOnUI(VisualElement ve) => SelectedUIElements.Remove(ve);

    #endregion
}