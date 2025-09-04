using Assets.Entity.DataContainers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentAnchor : MonoBehaviour
    {
        public int Index;
        public string ClassType;
        public int SizeType;
        public Vector2 RotationSector;
        public Vector2[] FireSectors;

        public bool CanBePlaced(Equipment equipment, int index)
        {
            EquipmentContainer equipmentContainer = equipment.EquipmentContainer;
            return equipmentContainer.General.Class == ClassType && Index == index &&
                   SizeType == equipmentContainer.General.SizeType;
        }

        public void SetTransform(Equipment equipment)
        {
            if (equipment == null) return;
            var eqTransform = equipment.transform;
            eqTransform.parent = transform;
            eqTransform.position = transform.position;
            eqTransform.rotation = transform.rotation;
        }
    }
}
