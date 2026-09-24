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
using UI.GUI.CommandLine;
using UnityEngine;
using UnityEngine.U2D;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility, IStats, IPoolInstance
    {
        [Header("Settings")]
        [SerializeField] private bool _isPlayerEntity;
        [SerializeField] private EntityNameplate _nameplate;

        public EntityData data;
        public EntityAssembler Assembler { get; private set; }
        public TotalAbbilitiesController TotalAbbilitiesController { get; private set; }
        public StatModController StatModController { get; private set; } = new();
        public BuffStatusesController Buffs { get; private set; }
        public EntityStatsAggregator AggregatedStats { get; private set; }
        public IDriver Driver { get; set; }
        public string Id { get; set; }
        [HideInInspector] public HullBase hull;

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        public bool IsInitialized { get; private set; } = false;
        private UniTaskCompletionSource _initTcs;

        private void Update()
        {
            if (hull == null) return;
            Driver?.UpdateControl();
        }

        #region Setup

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

        #region Enable/Disable
        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            ResetInitializationState();
        }

        #endregion

        private void Awake()
        {
            ResetInitializationState();
            Assembler = new EntityAssembler(this);
            TotalAbbilitiesController = new(this);

            Id = GameObjectHandler.GenerateUniqueId(name);
            if (_isPlayerEntity)
            {
                Driver = gameObject.AddComponent<PlayerController>();
                Driver.Setup(this);
                if (GameSessionHandler.Instance != null)
                    GameSessionHandler.Instance.playerController = this;
                Assembler.onSetHull += (HullBase hull) => {
                    if (hull != null && CameraController.Instance != null)
                        CameraController.Instance.Follow(hull.transform);
                };
            }
            AggregatedStats = new EntityStatsAggregator(this);
        }

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

            var aiDriver = gameObject.AddComponent<AiDriverController>();
            aiDriver.AddScripts(scripts?.ToArray() ?? new ScriptBase[0]);
            Driver = aiDriver;
            Driver.Setup(this);
        }

        private void SetupNameplate()
        {
            if (_nameplate == null) return;
            var sprites = hull.Sprites;
            if (sprites == null) { _nameplate.enabled = false; return; }

            float maxX = 0f;
            foreach (Sprite sprite in sprites.Select(s => s.sprite))
            {
                if (sprite == null) continue;
                Vector3 size = sprite.bounds.size;
                if (size.x > maxX) maxX = size.x;
            }

            _nameplate.transform.localScale = new Vector3(maxX, maxX, 0f);
        }
        #endregion

        #region IDriver Facade Methods

        public bool CanMove { get; set; } = true;
        public bool CanUseAbilities { get; set; } = true;

        public void Move(float acceleration, float rotationInput)
        {
            if (!CanMove) return;
            if (acceleration > 0) hull.AddSpeed(true);
            else if (acceleration < 0) hull.AddSpeed(false);
            hull.Movement(rotationInput);
        }

        public void AimAt(Vector2 worldPosition) => hull.RotateEquipment(worldPosition);

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
        public AbilitiesController abilitiesController;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit)) ;
        }

        #endregion

        #region IStats

        [SerializeField] private StatModController _statModController;

        private const StatLayer _statLayer = StatLayer.Hull;
        public float GetLifetimeStat(StatType type) => _statModController.GetStat(type, _statLayer);
        public IDataContainer GetInitialData() => data;

        public float GetTotalLifetimeStat(StatType type) => AggregatedStats.GetStat(type);

        #endregion

        #region Buffs


        #endregion

        #region IPoolInstance

        public void ReleaseToPool()
        {

        }

        #endregion
    }
}
