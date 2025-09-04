using System.Collections.Generic;
using Assets.Entity.DataContainers;
using Assets.Entity.Equipment;
using UnityEngine;

namespace Assets.Entity
{
    public class Hull : MonoBehaviour
    {
        [SerializeField] public HullContainer HullContainer;
        public List<EquipmentAnchor> EquipmentAnchors;
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
    }
}
