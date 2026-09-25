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
        #region Fields & Properties

        [Header("Components & State")]
        [field: SerializeField] public BuffStatusesController Buffs { get; protected set; }
        [SerializeField] protected StatModController statModController;

        public string Id { get; set; }
        public SpriteRenderer[] Sprites => _sprites;
        public GameObject GameObject => gameObject;
        public LayerType Layer => (LayerType)gameObject.layer;

        protected EntityController entityController;
        protected LocalAnimatorController animatorController;
        protected AbilitiesController abilitiesController;
        protected readonly ActionDataController actionDataController = new();

        private SpriteRenderer[] _sprites;

        protected abstract StatOptions StatOptions { get; }
        protected abstract StatLayer StatLayer { get; }

        public event Action OnGameObjectDestroyed;

        #endregion

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

            var triggerCollider = GameObjectHandler.DuplicateCollider2D(gameObject);
            if (triggerCollider != null) triggerCollider.isTrigger = true;
        }

        protected virtual void OnDestroy()
        {
            if (statModController != null) statModController.OnChange -= OnStatModChanged;
            if (entityController != null) entityController.OnHighlightStateChanged -= HandleHighlight;

            OnGameObjectDestroyed?.Invoke();
        }

        #endregion

        #region Setup & Initialization

        public virtual void Setup(EntityController entityController)
        {
            if (this.entityController != null) this.entityController.OnHighlightStateChanged -= HandleHighlight;

            this.entityController = entityController;

            if (this.entityController != null) this.entityController.OnHighlightStateChanged += HandleHighlight;

            _sprites = GameObjectHandler.GetNodesByType<SpriteRenderer>(transform).ToArray();
            Buffs = new BuffStatusesController(gameObject, statModController);

            var statOptions = StatOptions;

            StatModController parentModController = entityController != null ? entityController.StatModController : null;

            statModController = new StatModController(parentModController, statOptions);
            statModController.OnChange += OnStatModChanged;

            SetupInitialBuffs(statOptions.buffs);
        }

        protected virtual void OnStatModChanged()
        {
            actionDataController.MarkDirty();
            entityController?.AggregatedStats?.MarkDirty();
        }

        protected virtual void SetupInitialBuffs(IEnumerable<BuffStatus> buffs)
        {
            if (buffs == null) return;
            var snapshot = GetSnapshot();

            foreach (var buff in buffs)
            {
                if (buff.Scope == BuffScope.Global && entityController?.Buffs != null)
                {
                    entityController.Buffs.AddBuff(buff, snapshot);
                    OnGameObjectDestroyed += () => entityController.Buffs.RemoveBuff(buff.Id);
                }
                else Buffs?.AddBuff(buff, snapshot);
            }
        }

        #endregion

        #region IInteractive & IBuffable

        public virtual void AddBuff(InteractionContext context, BuffStatus buff)
        {
            if (buff == null) return;

            if (buff.Scope == BuffScope.Global && entityController?.Buffs != null)
                entityController.Buffs.AddBuff(buff, context.SourceSnapshot);
            else Buffs?.AddBuff(buff, context.SourceSnapshot);
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

        public float GetLifetimeStat(StatType type) => statModController != null ? statModController.GetStat(type, StatLayer) : 0f;
        public abstract IDataContainer GetInitialData();

        #endregion

        #region IAbbility

        public virtual IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController?.RuntimeAbilities;

        public virtual void AddAbility(AbilityUnit ability) => abilitiesController?.AddAbility(ability);

        public virtual bool RemoveAbility(AbilityUnit ability) => abilitiesController != null && abilitiesController.RemoveAbility(ability);

        public virtual void Activate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            if (abilitiesController != null && abilitiesController.TryActivate(targetPos, abilityUnit))
            {
                float activationRate = GetLifetimeStat(StatType.ActivationRate);
                animatorController?.PlayAction(abilityUnit.animationID, activationRate == 0 ? 1 : activationRate);
            }
        }

        public EntitySnapshot GetSnapshot() => entityController?.GetSnapshot();

        #endregion

        #region Hover & Highlight Logic

        protected virtual void OnMouseEnter()
        {
            Debug.Log("Moused");
            entityController?.SetHighlight(true);
        }
        protected virtual void OnMouseExit() => entityController?.SetHighlight(false);

        private void HandleHighlight(bool isHighlighted) => SetSpritesHighlight(isHighlighted);

        protected virtual void SetSpritesHighlight(bool isHighlighted)
        {
            if (_sprites == null || _sprites.Length == 0) return;

            Color targetColor = isHighlighted ? Color.yellow : Color.white;
            foreach (var sprite in _sprites)
                if (sprite != null) sprite.color = targetColor;
        }

        #endregion
    }
}