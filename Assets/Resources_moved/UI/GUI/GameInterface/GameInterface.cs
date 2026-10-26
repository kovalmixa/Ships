using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameInterface : UIWindow
{
    #region Fields

    [SerializeField] private Material _progressBarMaterial;

    #region ToolkitElements

    private VisualElement _miniMapVE;
    private Button _optionsButton;
    private Button _questsButton;
    private Button _inventoryButton;
    private Button _skillTreeButton;

    private VisualElement _dialogueWindow;
    private VisualElement _questWindow;

    private VisualElement _speedometerVE;
    private VisualElement _healthBarVE;
    private VisualElement _energyBarVE;
    private VisualElement _experienceBarVE;
    private Button[] _abilityButtons;

    #endregion

    #region ScriptObjects

    private ProgressBarTK _healthBarTK;
    private ProgressBarTK _energyBarTK;
    private ProgressBarTK _experienceBarTK;

    #endregion

    private readonly Dictionary<VisualElement, EventCallback<PointerEnterEvent>> _enterCallbacks = new();
    private readonly Dictionary<VisualElement, EventCallback<PointerLeaveEvent>> _leaveCallbacks = new();
    private readonly HashSet<VisualElement> _elementsInThisWindow = new();

    #endregion

    #region Setup & Lifecycle

    private void OnEnable()
    {
        if (RootVisualElement == null) return;

        _healthBarVE = RootVisualElement.Q<VisualElement>("HealthBar");
        _energyBarVE = RootVisualElement.Q<VisualElement>("EnergyBar");
        _experienceBarVE = RootVisualElement.Q<VisualElement>("ExperienceBar");

        if (_healthBarVE != null)
        {
            _healthBarTK = new ProgressBarTK();
            _healthBarTK.Initialize(_healthBarVE, _progressBarMaterial);
        }

        if (_energyBarVE != null)
        {
            _energyBarTK = new ProgressBarTK();
            _energyBarTK.Initialize(_energyBarVE, _progressBarMaterial);
        }

        if (_experienceBarVE != null)
        {
            _experienceBarTK = new ProgressBarTK();
            _experienceBarTK.Initialize(_experienceBarVE, _progressBarMaterial);
        }

        InitAbilityButtons();
        InitInterfaceButtons();
    }

    private void InitAbilityButtons()
    {
        var abilityButtonsList = new List<Button>();
        for (int i = 1; i <= 9; i++)
        {
            var btn = RootVisualElement.Q<Button>("AbButton" + i);
            if (btn != null) abilityButtonsList.Add(btn);
        }

        var btnZero = RootVisualElement.Q<Button>("AbButton0");
        if (btnZero != null) abilityButtonsList.Add(btnZero);

        var btnLb = RootVisualElement.Q<Button>("AbButtonLb");
        var btnRb = RootVisualElement.Q<Button>("AbButtonRb");
        if (btnLb != null) abilityButtonsList.Add(btnLb);
        if (btnRb != null) abilityButtonsList.Add(btnRb);

        _abilityButtons = abilityButtonsList.ToArray();
    }

    private void InitInterfaceButtons()
    {
        _optionsButton = RootVisualElement.Q<Button>("OptionsButton");
        _questsButton = RootVisualElement.Q<Button>("QuestsButton");
        _inventoryButton = RootVisualElement.Q<Button>("InventoryButton");
        _skillTreeButton = RootVisualElement.Q<Button>("SkillTreeButton");

        // Пример привязки кликов
        // if (_optionsButton != null) _optionsButton.clicked += OnOptionsClicked;
    }

    #endregion
}