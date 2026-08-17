using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.AI;
using Scripts;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility, IStats
    {
        public EntityData data;
        public EntityAssembler Assembler { get; private set; }
        public TotalAbbilitiesController totalAbbilitiesController { get; private set; }
        public StatModController statModController { get; private set; } = new();
        public BuffStatusesController Buffs { get; private set; }
        public IDriver Driver { get; set; }
        [SerializeField] private GameObject _despawnPrefab;
        public string Id { get; set; }
        [HideInInspector] public HullBase hull;

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        private void Update()
        {
            if (hull == null) return;
            Driver?.UpdateControl(this);
        }

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
                totalAbbilitiesController.Invoke(targetPosition, (WeaponType)action.ActionId);
            else if (action.Category == ActionCategory.Ability)
                totalAbbilitiesController.Invoke(targetPosition, (AbilityType)action.ActionId);
        }

        #endregion

        #region Setup

        private void Awake()
        {
            Assembler = new EntityAssembler(this);
            totalAbbilitiesController = new(this);

            Id = GameObjectHandler.GenerateUniqueId(name);
            if (GameObjectHandler.GetAI(this) == null)
            {
                Driver = gameObject.AddComponent<PlayerController>();
                GameObjectHandler.playerController = this;
                Assembler.onSetHull += (HullBase hull) => {
                    if (hull != null) CameraController.Instance.Follow(hull.transform);
                };
            }
        }

        public void Setup(EntityData data)
        {
            if (data == null) return;
            this.data = data;
            Assembler.Build(data);
        }

        public void SetupAi(params ScriptBase[] scripts)
        {
            if (Driver is PlayerController) return;
            Driver = gameObject.AddComponent<AiController>();
            if (Driver is AiController aiController)
            {
                aiController.Scripts = new Queue<ScriptBase>(scripts);
            }
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
    }
}
