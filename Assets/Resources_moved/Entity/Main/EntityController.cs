using AI;
using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UI.GUI.EntityGUI;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility, IStats, IPoolInstance
    {
        #region Fields & Properties

        [Header("UI Components")]
        [SerializeField] private EntityNameplate _nameplate;

        [Header("Data")]
        public EntityData data;

        public string Id { get; set; }
        public HullBase Hull { get; set; }
        public IDriver Driver { get; set; }
        public EntityAssembler Assembler { get; private set; }
        public TotalAbbilitiesController TotalAbbilitiesController { get; private set; }
        public StatModController StatModController { get; private set; } = new();
        public BuffStatusesController Buffs { get; private set; }
        public EntityStatsAggregator AggregatedStats { get; private set; }
        public AbilitiesController AbilitiesController { get; private set; }

        public bool IsInitialized { get; private set; } = false;
        public bool CanMove { get; set; } = true;
        public bool CanUseAbilities { get; set; } = true;

        private UniTaskCompletionSource _initTcs;
        private const StatLayer _hullStatLayer = StatLayer.Hull;

        public event Action<bool> OnHighlightStateChanged;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            ResetInitializationState();
            Assembler = new EntityAssembler(this);
            TotalAbbilitiesController = new TotalAbbilitiesController(this);

            Id = GameObjectHandler.GenerateUniqueId(name);
            AggregatedStats = new EntityStatsAggregator(this);
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            ResetInitializationState();
        }

        private void Update()
        {
            if (Hull == null) return;
            Driver?.UpdateControl();
        }

        #endregion

        #region Setup & Initialization

        #region Init Token API

        public UniTask WaitUntilInitializedAsync(CancellationToken token = default)
        {
            if (IsInitialized) return UniTask.CompletedTask;
            return _initTcs.Task.AttachExternalCancellation(token);
        }

        public void ResetInitializationState()
        {
            IsInitialized = false;
            _initTcs = new UniTaskCompletionSource();
        }

        private void InvokeInitializationState()
        {
            IsInitialized = true;
            _initTcs?.TrySetResult();
        }

        #endregion

        public async UniTask Setup(EntityData data)
        {
            try
            {
                if (data == null)
                {
                    InvokeInitializationState();
                    return;
                }
                this.data = data;
                SetupDriver();
                await Assembler.Build(data);

                SetupNameplate();

                InvokeInitializationState();
            }
            catch (Exception ex)
            {
                _initTcs?.TrySetException(ex);
                throw;
            }
        }

        public async UniTask Setup(EntityData data, IEnumerable<ScriptBase> scripts = null)
        {
            if (data == null) return;

            await Setup(data);
            if (Driver is AiDriverController aiDriver)
            {
                aiDriver.AddScripts(scripts?.ToArray() ?? Array.Empty<ScriptBase>());
            }
        }

        private void SetupDriver()
        {
            if (data.isPlayer)
            {
                Driver = gameObject.AddComponent<PlayerController>();
                Assembler.onSetHull += (HullBase newHull) =>
                {
                    if (newHull != null && CameraController.Instance != null)
                        CameraController.Instance.Follow(newHull.transform);
                };
            }
            else
            {
                Driver = gameObject.AddComponent<AiDriverController>();
            }

            Driver.Setup(this);
        }

        private void SetupNameplate()
        {
            if (_nameplate == null || Hull == null) return;
            var sprites = Hull.Sprites;
            if (sprites == null || !sprites.Any() || data.isPlayer)
            {
                _nameplate.gameObject.SetActive(false);
                return;
            }
            _nameplate.Setup(this);

            float maxX = 0f;
            foreach (Sprite sprite in sprites.Select(s => s.sprite))
            {
                if (sprite == null) continue;
                Vector3 size = sprite.bounds.size;
                if (size.x > maxX) maxX = size.x;
            }

            float calculatedBasicScale = maxX / _nameplate.standartSize;
            _nameplate.transform.localScale = new Vector2(calculatedBasicScale, calculatedBasicScale);
            _nameplate._offset = new Vector3(0, 1.5f * calculatedBasicScale, 0);
        }

        #endregion

        #region IDriver Facade Methods

        public void Move(float acceleration, float rotationInput)
        {
            if (!CanMove || Hull == null) return;
            if (acceleration > 0) Hull.AddSpeed(true);
            else if (acceleration < 0) Hull.AddSpeed(false);
            Hull.Movement(rotationInput);
        }

        public void AimAt(Vector2 worldPosition)
        {
            if (Hull != null) Hull.RotateEquipment(worldPosition);
        }

        public void ExecuteAction(KeyAction action, Vector2 targetPosition)
        {
            if (!CanUseAbilities || !IsInitialized) return;

            if (action.Category == ActionCategory.Weapon)
                TotalAbbilitiesController.Invoke(targetPosition, (WeaponType)action.ActionId);
            else if (action.Category == ActionCategory.Ability)
                TotalAbbilitiesController.Invoke(targetPosition, (AbilityType)action.ActionId);
        }

        #endregion

        #region IAbbility

        public GameObject GameObject => gameObject;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => AbilitiesController?.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => AbilitiesController?.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => AbilitiesController != null && AbilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            AbilitiesController?.TryActivate(targetPos, abilityUnit);
        }

        #endregion

        #region IStats

        public float GetLifetimeStat(StatType type) => StatModController.GetStat(type, _hullStatLayer);
        public IDataContainer GetInitialData() => data;
        public float GetTotalLifetimeStat(StatType type) => AggregatedStats.GetStat(type);

        #endregion

        #region IPoolInstance

        public void ReleaseToPool()
        {
            // Логика сброса объекта в пул
        }

        #endregion

        #region Triggers & Visuals

        public void SetHighlight(bool isHighlighted)
        {
            OnHighlightStateChanged?.Invoke(isHighlighted);
            Debug.Log("Highlighted");
            if (_nameplate == null) return;
            if (isHighlighted) _nameplate.Show();
            else _nameplate.Hide();
        }

        #endregion

        #region Helpers

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        #endregion
    }
}