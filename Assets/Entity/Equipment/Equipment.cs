using System;
using System.Collections;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Actions;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject
    {
        public Entity Entity { get; set; }
        public HullEquipmentProperties HullEquipmentProperties { set; get; }
        private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer
        {
            get => _equipmentContainer;
            set
            {
                _equipmentContainer = value;
                string[] texturePaths = _equipmentContainer.Graphics.Textures;
                lastActtivationTimes = new float[_equipmentContainer.OnActivate.Length];
                for (int i = 0; i < lastActtivationTimes.Length; i++)
                    lastActtivationTimes[i] = -Mathf.Infinity;
                StartCoroutine(SetupTextureLayers(texturePaths));
            }
        }
        public int LayerIndex;
        private float[] lastActtivationTimes;
        private IEnumerator SetupTextureLayers(string[] texturePaths)
        {
            IsComplexCollision = true;
            yield return StartCoroutine(SetupLayersCoroutine(texturePaths));
            foreach (Transform child in LayersAnchor.GetComponentsInChildren<Transform>())
            {
                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.sortingOrder += LayerIndex + 1;
            }
            Quaternion rotation = transform.rotation;
            rotation.z = HullEquipmentProperties.Rotation;
            transform.rotation = rotation;
        }
        private void Awake()
        {
            IsComplexCollision = true;
        }
        public new string Type
        {
            get => EquipmentContainer.General.SizeType;
            set => EquipmentContainer.General.SizeType = value;
        }
        public void Rotate(float angle)
        {
            if (EquipmentContainer == null) return;
            if (!CanRotate()) return;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            Quaternion rotationStep = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _equipmentContainer.Physics.RotationSpeed * Time.deltaTime
            );
            float resultWorldAngle = NormalizeAngle(rotationStep.eulerAngles.z);
            float hullRotation = NormalizeAngle(Entity.transform.eulerAngles.z);
            float baseRotation = NormalizeAngle(HullEquipmentProperties.Rotation);
            float resultLocalAngle = NormalizeAngle(resultWorldAngle - hullRotation - baseRotation);
            Vector2 sector = HullEquipmentProperties.RotationSector;
            if (IsAngleWithinSector(resultLocalAngle, sector.x, sector.y))
            {
                transform.rotation = rotationStep;
            }
        }
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
        private bool IsAngleWithinSector(float angle, float min, float max) => min <= angle && angle <= max;
        public bool CanRotate()
        {
            if (HullEquipmentProperties == null) return false;
            return HullEquipmentProperties.RotationSector != null;
        }
        public void Activate(Vector3 position)
        {
            if (EquipmentContainer == null) return;
            float time = Time.time;
            int length = EquipmentContainer.OnActivate.Length;
            for (int i = 0; i < length; i++)
            {
                ActivationContainer activation = EquipmentContainer.OnActivate[i];
                if (!IsActivationWithinSector(position, activation)) continue;
                if (time - lastActtivationTimes[i] >= activation.Delay)
                {
                    ActionContext actionContext = FormActionContext(activation, position);
                    ActionHandler.Execute(activation.Type, actionContext);
                    lastActtivationTimes[i] = time;
                }
            }
        }
        private bool IsActivationWithinSector(Vector3 position, ActivationContainer activation)
        {
            Vector2 direction = position - transform.position;
            float targetLocalAngle = Vector2.SignedAngle(transform.up, direction);
            if (Mathf.Abs(targetLocalAngle) >= 12.5f / activation.Delay) return false;
            if (activation.Delay <= 1f) return true;
            Vector2 sector = HullEquipmentProperties.FireSector;
            return IsAngleWithinSector(targetLocalAngle, sector.x, sector.y); ;
        }
        private ActionContext FormActionContext(ActivationContainer activation, Vector3 position)
        {
            ActionContext actionContext = new();
            actionContext.ObjectId = activation.Projectile;
            actionContext.Source = gameObject;
            actionContext.TargetPosition = position;
            actionContext.Position = activation.Position;
            //actionContext.AmountValue = ?? // для добавления значения к абилкам или хп, мп и так далее
            return actionContext;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
    }
}
