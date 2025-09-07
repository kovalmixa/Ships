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
        public int OrderLayer;

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
            Vector2 scale = eqTransform.localScale;
            eqTransform.SetParent(transform, false);
            eqTransform.position = transform.position;
            eqTransform.rotation = transform.rotation;
            eqTransform.localScale = scale;

            equipment.SetRenderLayerOrder(OrderLayer);
        }
    }
}
