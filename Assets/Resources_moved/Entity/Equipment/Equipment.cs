using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Entity.Equipment
{
    public class Equipment : EntityPartBase
    {
        #region Fields & Properties

        [Header("Data & Configuration")]
        [field: SerializeField] public EquipmentDataSO Data { get; private set; }
        public EquipmentAnchor EquipmentAnchor { get; set; }

        [Header("Editor Settings")]
        [Tooltip("Drag shot/ability nodes here directly from the object hierarchy")]
        [SerializeField] private List<Transform> _abilityNodes = new();

        private const float _basicAngle = 90f;
        private float _currentLocalAngle = 0f;
        private bool _isLocalAngleInitialized = false;

        protected override StatOptions StatOptions => Data != null ? Data.statOptions : default;
        protected override StatLayer StatLayer => StatLayer.Equipment;

        public Vector2 Position => entityController != null
            ? (Vector2)transform.position + (Vector2)entityController.transform.position
            : transform.position;

        #endregion

        #region Setup & Initialization

        public override void Setup(EntityController entityController)
        {
            base.Setup(entityController);

            abilitiesController = new EqAbilitiesController(
                StatOptions.abilities,
                entityController.TotalAbbilitiesController,
                actionDataController,
                this,
                _basicAngle,
                EquipmentAnchor
            );

            OnGameObjectDestroyed += () => abilitiesController?.RemoveAbilities();
        }

        #endregion

        #region Rotation Logic

        public void Rotate(Vector3 targetPos)
        {
            if (Data == null || !CanRotate()) return;

            float rotationSpeed = GetLifetimeStat(StatType.RotationSpeed);

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
            if (targetOffset <= sectorWidth)
            {
                desiredOffset = targetOffset;
            }
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

        public bool CanRotate()
        {
            return EquipmentAnchor != null && EquipmentAnchor.rotationSector != Vector2.zero;
        }

        private float NormalizeAngle(float angle)
        {
            float result = angle % 360f;
            if (result < 0) result += 360f;
            return result;
        }

        #endregion

        #region Helpers & Overrides

        public override IDataContainer GetInitialData() => Data;

        #endregion

        #region Editor Context Menu & Gizmos

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
    }
}