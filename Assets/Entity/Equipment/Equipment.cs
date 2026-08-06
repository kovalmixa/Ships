using Assets.Common;
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
using Assets.Common.Interfaces;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IInteractive, IStats, IAbbility, IBuffable
    {
        private EntityController _entityController;
        public BuffStatusesController Buffs { get; private set; }
        [field: SerializeField] public EquipmentDataSO Data { get; private set; }
        public EquipmentAnchor EquipmentAnchor { get; set; }
        private const float _basicAngle = 90;

        [Header("Editor Settings")]
        [Tooltip("Drag shot/ability nodes here directly from the object hierarchy")]
        [SerializeField] private List<Transform> _abilityNodes = new();

        public Vector2 Position
        {
            get => transform.position + _entityController.transform.position;
            set { }
        }
        public string Id { get; set; }
        public event Action OnGameObjectDestroyed;

        #region Editor

        private void OnValidate()
        {
            if (Data == null) return;

            var statOptions = Data.statOptions;
            if (statOptions.stats != null) foreach (var stat in statOptions.stats) stat?.UpdateInspectorName();
            if (statOptions.mods != null) foreach (var mod in statOptions.mods) mod?.UpdateInspectorName();
        }

#if UNITY_EDITOR
        [ContextMenu("Bake node coordinates into SO")]
        public void BakeNodesToSO()
        {
            if (Data == null || Data.statOptions.abilities == null)
            {
                Debug.LogError($"[{name}] Data or Abilities are not assigned!");
                return;
            }

            var abilities = Data.statOptions.abilities;
            if (abilities.Count == 0)
            {
                Debug.LogWarning($"[{name}] There are no abilities in the SO!");
                return;
            }

            Undo.RecordObject(Data, "Bake Ability Positions");

            for (int i = 0; i < abilities.Count; i++)
            {
                if (i >= _abilityNodes.Count || _abilityNodes[i] == null)
                {
                    Debug.LogWarning($"[{name}] No Transform node assigned for ability #{i} in the Ability Nodes array.");
                    continue;
                }

                Vector3 localPos = transform.InverseTransformPoint(_abilityNodes[i].position);

                var ability = abilities[i];
                ability.abilityPosition = new Vector2(localPos.x, localPos.y);
                abilities[i] = ability;
            }

            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>[Success]</color> Node coordinates baked into SO for {Data.name}!");
        }

        private void OnDrawGizmosSelected()
        {
            if (Data == null || Data.statOptions.abilities == null) return;

            Gizmos.color = Color.red;
            foreach (var ability in Data.statOptions.abilities)
            {
                Vector3 worldPos = transform.TransformPoint(ability.abilityPosition);
                Gizmos.DrawSphere(worldPos, 0.05f);
            }
        }
#endif

        #endregion

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
        }

        public void Setup(EntityController entityController)
        {
            _entityController = entityController;
            Buffs = new BuffStatusesController(gameObject, _statModController);
            var snapshot = GetSnapshot();
            var statOptions = Data.statOptions;
            _statModController = new(entityController.statModController, statOptions);
            _statModController.OnChange += () => _actionDataController.MarkDirty();

            foreach (var buff in statOptions.buffs)
            {
                if (buff.Scope == BuffScope.Global)
                {
                    entityController.Buffs.AddBuff(buff, snapshot);
                    OnGameObjectDestroyed += () => entityController.Buffs.RemoveBuff(buff.Id);
                }
                else Buffs.AddBuff(buff, snapshot);
            }

            abilitiesController = new(statOptions.abilities, _entityController.totalAbbilitiesController,
                _actionDataController, this, _basicAngle, EquipmentAnchor);
            OnGameObjectDestroyed += () => abilitiesController.RemoveAbilities();
        }

        private void OnDestroy() => OnGameObjectDestroyed?.Invoke();

        #endregion

        #region Rotation

        private float _currentLocalAngle = 0f;
        private bool _isLocalAngleInitialized = false;

        public void Rotate(Vector3 targetPos)
        {
            if (Data == null || !CanRotate()) return;

            var rotationSpeed = GetLifetimeStat(StatType.RotationSpeed);

            Vector3 localTarget = EquipmentAnchor.transform.InverseTransformPoint(targetPos);
            float targetAngle = Mathf.Atan2(localTarget.y, localTarget.x) * Mathf.Rad2Deg;

            float min = EquipmentAnchor.rotationSector.x;
            float max = EquipmentAnchor.rotationSector.y;

            if (!_isLocalAngleInitialized)
            {
                _currentLocalAngle = transform.localEulerAngles.z + _basicAngle;
                _isLocalAngleInitialized = true;
            }

            if (Mathf.Abs(max - min) >= 360f)
            {
                Quaternion targetLocalRot = Quaternion.Euler(0f, 0f, targetAngle - _basicAngle);
                transform.localRotation = Quaternion.RotateTowards(
                    transform.localRotation,
                    targetLocalRot,
                    rotationSpeed * Time.deltaTime
                );
                _currentLocalAngle = transform.localEulerAngles.z + _basicAngle;
                return;
            }

            float sectorWidth = max - min;

            float currentOffset = NormalizeAngle(_currentLocalAngle - min);
            float targetOffset = NormalizeAngle(targetAngle - min);

            float desiredOffset;
            if (targetOffset <= sectorWidth) desiredOffset = targetOffset;
            else
            {
                float distToMin = 360f - targetOffset;
                float distToMax = targetOffset - sectorWidth;
                desiredOffset = (distToMin < distToMax) ? 0f : sectorWidth;
            }

            float newOffset = Mathf.MoveTowards(currentOffset, desiredOffset, rotationSpeed * Time.deltaTime);
            newOffset = Mathf.Clamp(newOffset, 0f, sectorWidth);

            _currentLocalAngle = min + newOffset;
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentLocalAngle - _basicAngle);
        }

        private float NormalizeAngle(float angle)
        {
            float result = angle % 360f;
            if (result < 0) result += 360f;
            return result;
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.rotationSector != Vector2.zero;
        }

        #endregion

        #region IInteractive

        public LayerType Layer => (LayerType)gameObject.layer;
        public GameObject GameObject => gameObject;

        public void AddBuff(InteractionContext context, BuffStatus buff)
        {
            if (buff == null) return;
            if (buff.Scope == BuffScope.Global) _entityController.Buffs.AddBuff(buff, context.SourceSnapshot);
            else Buffs.AddBuff(buff, context.SourceSnapshot);
        }

        public void TakeDamage(InteractionContext context, DamageData data)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(InteractionContext context, HealData data)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IStats

        [SerializeField] private StatModController _statModController;

        private const StatLayer _statLayer = StatLayer.Equipment;

        public float GetLifetimeStat(StatType type) => _statModController.GetStat(type, _statLayer);

        public IDataContainer GetInitialData() => Data;

        #endregion

        #region IAbbility

        public EqAbilitiesController abilitiesController;
        private readonly ActionDataController _actionDataController = new();

        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit)) ;
        }

        public EntitySnapshot GetSnapshot() => _entityController.GetSnapshot();

        #endregion
    }
}