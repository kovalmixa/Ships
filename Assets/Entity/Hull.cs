using System.Collections.Generic;
using Assets.Entity.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using UnityEngine;

namespace Assets.Entity
{
    public class Hull : InGameObject, IDamageable, IActivation
    {
        public float MaxSpeed = 5f;
        public float Acceleration = 3f;
        public float RotationSpeed = 60f;

        public float CurrentSpeed { get; set; }
        public float Speed { get; set; }

        [SerializeField] public HullContainer HullContainer;
        public List<EquipmentAnchor> EquipmentAnchors;
        public ActivationContainer[] Activations { get; set; }
        private void Awake()
        {
            CollectAnchors(transform);
        }

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                var equipmentAnchor = child.GetComponent<EquipmentAnchor>();
                if (equipmentAnchor != null)
                {
                    EquipmentAnchors.Add(equipmentAnchor);
                }
                CollectAnchors(child);
            }
        }

        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }
        public void Activate(Vector3 targetPosition, string type = null)
        {
            throw new System.NotImplementedException();
        }

    }
}
