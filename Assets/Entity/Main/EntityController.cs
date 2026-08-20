using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Markers.Spawner;
using AI;
using Scripts;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility, IStats, IPoolInstance
    {
        [Header("Settings")]
        [SerializeField] private bool _isPlayerEntity;
        public EntityData data;
        public EntityAssembler Assembler { get; private set; }
        public TotalAbbilitiesController TotalAbbilitiesController { get; private set; }
        public StatModController StatModController { get; private set; } = new();
        public BuffStatusesController Buffs { get; private set; }
        public IDriver Driver { get; set; }
        public string Id { get; set; }
        [HideInInspector] public HullBase hull;

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        private void Update()
        {
            if (hull == null) return;
            Driver?.UpdateControl(this);
        }
        
        #region Setup

        private void Awake()
        {
            Assembler = new EntityAssembler(this);
            TotalAbbilitiesController = new(this);

            Id = GameObjectHandler.GenerateUniqueId(name);
            if (_isPlayerEntity)
            {
                Driver = gameObject.AddComponent<PlayerController>();
                SceneController.Instance.playerController = this;
                Assembler.onSetHull += (HullBase hull) => {
                    if (hull != null) CameraController.Instance.Follow(hull.transform);
                };
            }
        }

        public async Task Setup(EntityData data)
        {
            if (data == null) return;
            this.data = data;
            await Assembler.Build(data);
        }

        public async Task Setup(NpcData data, IEnumerable<ScriptBase> scripts = null)
        {
            if (data == null) return;
            await Setup(data.entityData);

            Driver = new AiDriverController();
            (Driver as AiDriverController).Scripts = (Queue<ScriptBase>)scripts;
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
            if (!CanUseAbilities) return;
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
