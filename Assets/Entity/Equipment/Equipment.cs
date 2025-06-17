using System.Collections;
using Assets.Entity.DataContainers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject
    {
        public HullEquipmentProperties HullEquipmentProperties { set; get; }
        private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer
        {
            get => _equipmentContainer;
            set
            {
                _equipmentContainer = value;
                string[] texturePaths = _equipmentContainer.Graphics.Textures;
                StartCoroutine(SetupTextureLayers(texturePaths));
                
            }
        }
        public int LayerIndex;
        private IEnumerator SetupTextureLayers(string[] texturePaths)
        {
            yield return StartCoroutine(SetupLayersCoroutine(texturePaths));
            foreach (Transform child in LayersAnchor.GetComponentsInChildren<Transform>())
            {
                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.sortingOrder += LayerIndex;
            }
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
            if (!CanRotate()) return;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle -= 90f);
            Quaternion rotationAngle = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _equipmentContainer.Physics.RotationSpeed * Time.deltaTime
            );
            float targetAngle = rotationAngle.eulerAngles.z;
            targetAngle = targetAngle > 180 ? targetAngle - 360 : targetAngle;
            Vector2 rotationSector = HullEquipmentProperties.RotationSector;
            if (targetAngle >= rotationSector.x && targetAngle <= rotationSector.y)
            {
                transform.rotation = rotationAngle;
            }
        }

        public bool CanRotate()
        {
            if (HullEquipmentProperties == null) return false;
            return HullEquipmentProperties.RotationSector != null;
        }
        public void Activate()
        {
            //code for activation
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
    }
}
