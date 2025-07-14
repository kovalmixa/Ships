using System;
using System.Collections;
using Assets.Entity.DataContainers;
using Assets.Handlers;
using Assets.InGameMarkers.Actions;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject, IActivation
    {
        private Activator _activator;
        public EntityBody EntityBody { get; set; }
        public HullEquipmentProperties HullEquipmentProperties { set; get; }
        private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer
        {
            get => _equipmentContainer;
            set
            {
                _equipmentContainer = value;
                _activator.SetActivations(_equipmentContainer.OnActivate);
                _activator.HostFireSectors = HullEquipmentProperties.FireSectors;
                string[] texturePaths = _equipmentContainer.Graphics.Textures;
                StartCoroutine(SetupTextureLayers(texturePaths));
            }
        }
        public ActivationContainer[] Activations 
        { 
            get => EquipmentContainer.OnActivate;
            set => EquipmentContainer.OnActivate = value;
        }

        public int LayerIndex;
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
            _activator = gameObject.AddComponent<Activator>();
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
            float resultWorldAngle = FunctionHandler.NormalizeAngle(rotationStep.eulerAngles.z);
            float hullRotation = FunctionHandler.NormalizeAngle(EntityBody.transform.eulerAngles.z);
            float baseRotation = FunctionHandler.NormalizeAngle(HullEquipmentProperties.Rotation);
            float resultLocalAngle = FunctionHandler.NormalizeAngle(resultWorldAngle - hullRotation - baseRotation);
            Vector2 sector = HullEquipmentProperties.RotationSector;
            if (IsAngleWithinSector(resultLocalAngle, sector.x, sector.y))
            {
                transform.rotation = rotationStep;
            }
        }
        private bool IsAngleWithinSector(float angle, float min, float max) => min <= angle && angle <= max;
        public bool CanRotate()
        {
            if (HullEquipmentProperties == null) return false;
            return HullEquipmentProperties.RotationSector != null;
        }
        public void Activate(Vector3 position, string type = null) =>_activator.TryActivate(position, type);
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
    }
}
