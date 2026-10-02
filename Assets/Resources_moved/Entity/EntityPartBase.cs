using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.BuffStatuses;
using Assets.Entity.Controllers;
using Assets.Entity.Interfaces;
using Assets.Entity.StatMods;
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
    public abstract class EntityPartBase : MonoBehaviour,
        IInteractive, IStats, IAbbility, IBuffable
    {
        #region Fields & Properties

        [Header("Components & State")]
        [field: SerializeField] public BuffStatusesController Buffs { get; protected set; }
        [SerializeField] protected StatModController statModController;
        public StatModController StatModController => statModController;
        public string Id { get; set; }
        public SpriteRenderer[] Sprites => sprites;
        public GameObject GameObject => gameObject;
        public LayerType Layer => (LayerType)gameObject.layer;

        protected EntityController entityController;
        protected LocalAnimatorController animatorController;
        protected AbilitiesController abilitiesController;
        protected readonly ActionDataController actionDataController = new();

        protected SpriteRenderer[] sprites;
        protected Collider2D partCollider;
        private bool _isMouseHovered = false;

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
            partCollider = GetComponent<Collider2D>();
        }

        protected virtual void Update()
        {
            CheckMouseHover();
        }

        protected virtual void OnDisable()
        {
            if (_isMouseHovered)
            {
                _isMouseHovered = false;
                entityController?.SetHighlight(false);
            }
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

            sprites = GameObjectHandler.GetNodesByType<SpriteRenderer>(transform).ToArray();
            Buffs = new BuffStatusesController(gameObject, statModController);

            var statOptions = StatOptions;

            statModController = new StatModController(this.entityController, statOptions);
            statModController.OnChange += OnStatModChanged;

            SetupInitialBuffs(statOptions.buffs);
            SetupStatValues();
        }

        private void SetupStatValues()
        {
            float maxHp = GetLifetimeStatValue(StatType.MaxHp);
            if (maxHp != 0) SetStatValue(StatType.Hp, maxHp);
            float maxEnergy = GetLifetimeStatValue(StatType.MaxEnergy);
            if (maxEnergy != 0) SetStatValue(StatType.Energy, maxEnergy);
        }

        protected virtual void OnStatModChanged()
        {
            actionDataController.MarkDirty();
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

        public float GetLifetimeStatValue(StatType type)
        {
            return statModController != null ? statModController.GetStatValue(type, StatLayer) : 0f;
        }

        public void SetStatValue(StatType type, float value)
        {
            statModController.SetStatValue(type, layer: StatLayer, value);
        }

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
                float activationRate = GetLifetimeStatValue(StatType.ActivationRate);
                animatorController?.PlayAction(abilityUnit.animationID, activationRate == 0 ? 1 : activationRate);
            }
        }

        public EntitySnapshot GetSnapshot() => entityController?.GetSnapshot();

        #endregion

        #region Hover & Highlight Logic

        private void CheckMouseHover()
        {
            if (partCollider == null) return;

            var cameraController = CameraController.Instance;
            if (cameraController == null || cameraController.Camera == null) return;

            Vector2 mouseWorldPos = cameraController.Camera.ScreenToWorldPoint(Input.mousePosition);
            bool isInside = partCollider.OverlapPoint(mouseWorldPos);

            if (isInside && !_isMouseHovered)
            {
                _isMouseHovered = true;
                entityController?.SetHighlight(true);
            }
            else if (!isInside && _isMouseHovered)
            {
                _isMouseHovered = false;
                entityController?.SetHighlight(false);
            }
        }

        private void HandleHighlight(bool isHighlighted) => SetSpritesHighlight(isHighlighted);

        protected virtual void SetSpritesHighlight(bool isHighlighted)
        {
            if (sprites == null || sprites.Length == 0) return;

            Color softYellow = new Color(1f, 0.98f, 0.8f);
            Color targetColor = isHighlighted ? softYellow : Color.white;
            foreach (var sprite in sprites) if (sprite != null) sprite.color = targetColor;
        }

        #endregion
    }
}