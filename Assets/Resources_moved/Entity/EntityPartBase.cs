using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.BuffStatuses;
using Assets.Entity.Controllers;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Entity.Controllers;
using GameplayActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Common
{
    public abstract class EntityPartBase : MonoBehaviour, IInteractive, IStats, IAbbility, IBuffable
    {
        [field: SerializeField] public BuffStatusesController Buffs { get; protected set; }
        [SerializeField] protected StatModController _statModController;
        private SpriteRenderer[] _sprites;
        public SpriteRenderer[] Sprites => _sprites;

        public string Id { get; set; }
        public event Action OnGameObjectDestroyed;

        protected EntityController entityController;
        protected LocalAnimatorController animatorController;
        protected readonly ActionDataController _actionDataController = new();
        protected AbilitiesController abilitiesController;

        protected abstract StatOptions StatOptions { get; }
        protected abstract StatLayer StatLayer { get; }
        public abstract IDataContainer GetInitialData();

        #region Unity Lifecycle & Editor

        protected virtual void OnValidate()
        {
            var statOptions = StatOptions;
            if (statOptions.stats != null)
                foreach (var stat in statOptions.stats) stat?.UpdateInspectorName();

            if (statOptions.mods != null)
                foreach (var mod in statOptions.mods) mod?.UpdateInspectorName();
        }

        protected virtual void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            animatorController = gameObject.AddComponent<LocalAnimatorController>();
        }

        protected virtual void OnDestroy() => OnGameObjectDestroyed?.Invoke();

        #endregion

        #region Setup

        public virtual void Setup(EntityController entityController)
        {
            this.entityController = entityController;
            _sprites = GameObjectHandler.GetNodesByType<SpriteRenderer>(transform).ToArray();
            Buffs = new BuffStatusesController(gameObject, _statModController);

            var statOptions = StatOptions;
            _statModController = new StatModController(entityController.StatModController, statOptions);
            _statModController.OnChange += OnStatModChanged;

            SetupInitialBuffs(statOptions.buffs);
        }

        protected virtual void OnStatModChanged() {
            _actionDataController.MarkDirty();
            entityController.AggregatedStats.MarkDirty();
        }

        protected virtual void SetupInitialBuffs(IEnumerable<BuffStatus> buffs)
        {
            if (buffs == null) return;
            var snapshot = GetSnapshot();

            foreach (var buff in buffs)
            {
                if (buff.Scope == BuffScope.Global)
                {
                    entityController.Buffs.AddBuff(buff, snapshot);
                    OnGameObjectDestroyed += () => entityController.Buffs.RemoveBuff(buff.Id);
                }
                else Buffs.AddBuff(buff, snapshot);
            }
        }

        #endregion

        #region IInteractive & IBuffable

        public LayerType Layer => (LayerType)gameObject.layer;
        public GameObject GameObject => gameObject;

        public virtual void AddBuff(InteractionContext context, BuffStatus buff)
        {
            if (buff == null) return;

            if (buff.Scope == BuffScope.Global)
                entityController.Buffs.AddBuff(buff, context.SourceSnapshot);
            else Buffs.AddBuff(buff, context.SourceSnapshot);
        }

        public virtual void TakeDamage(InteractionContext context, DamageData data)
        {
            Debug.Log($"Damaged with value {data.value} to {gameObject.name}");

        }

        public virtual void TakeHeal(InteractionContext context, HealData data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStats

        public float GetLifetimeStat(StatType type) => _statModController.GetStat(type, StatLayer);

        #endregion

        #region IAbbility

        public virtual IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;
        public virtual void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);
        public virtual bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public virtual void Activate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit))
            {
                float activationRate = GetLifetimeStat(StatType.ActivationRate);
                animatorController?.PlayAction(abilityUnit.animationID, activationRate == 0 ? 1 : activationRate);
            }
        }

        public EntitySnapshot GetSnapshot() => entityController.GetSnapshot();

        #endregion
    }
}