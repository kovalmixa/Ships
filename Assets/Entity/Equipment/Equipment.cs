using Assets.Entity.DataContainers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject
    {
        private EquipmentContainer _equipmentContainer;

        public EquipmentContainer EquipmentContainer
        {
            get => _equipmentContainer;
            set
            {
                _equipmentContainer = value;
                string[] texturePaths = _equipmentContainer.Graphics.Textures;
                StartCoroutine(SetupLayersCoroutine(texturePaths));
            }
        }

        private void Awake()
        {
            IsComplexCollision = true;
        }
        public bool CanBeRotatedByControl { get; set; }
        public new string Type { get; set; }
        public void Rotate(Angle angle)
        {
            if (!CanBeRotatedByControl) return;
            //code for rotation
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
