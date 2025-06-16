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
        public new string Type { get; set; }

        public void Rotate(float angle)
        {
            angle -= 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _equipmentContainer.Physics.RotationSpeed * Time.deltaTime
            );
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
