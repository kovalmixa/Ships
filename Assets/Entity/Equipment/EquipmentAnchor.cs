using System.Linq;
using Assets.DataContainers;
using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    [ExecuteInEditMode]
    public class EquipmentAnchor : MonoBehaviour
    {
        public int Index;
        public string ClassType;
        public int SizeType;
        public Vector2 RotationSector;
        public Vector2[] ActivationSectors;
        public int OrderLayer;
        public const bool IsStatic = false;

        public bool CanBePlaced(Equipment equipment, int index)
        {
            if (IsStatic) return false;
            EquipmentContainer equipmentContainer = equipment.EquipmentContainer;
            var eqClass = equipmentContainer.General.Class;
            var masterType = TypeListHandler.TryGetMasterType(eqClass);
            if (SizeType == equipmentContainer.General.SizeType && Index == index)
            {
                if (masterType == null)
                    return eqClass == ClassType;
                if (masterType == ClassType)
                    return true;
            }
            return false;
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
            equipment.EquipmentAnchor = this;
            GameObjectHandler.SetRenderLayerOrder(gameObject, OrderLayer);
        }

        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;
            Gizmos.color = Color.yellow;
            DrawSector(origin, RotationSector, 2f);
            if (ActivationSectors != null)
            {
                Gizmos.color = Color.red;
                foreach (var sector in ActivationSectors)
                {
                    DrawSector(origin, sector, 3f);
                }
            }
        }

        private void DrawSector(Vector3 origin, Vector2 sector, float radius)
        {
            float startAngle = sector.x;
            float endAngle = sector.y;
            int segments = 20;

            Vector3 prevPoint = origin + DirFromAngle(startAngle) * radius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, i / (float)segments);
                Vector3 newPoint = origin + DirFromAngle(angle) * radius;
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
            Gizmos.DrawLine(origin, origin + DirFromAngle(startAngle) * radius);
            Gizmos.DrawLine(origin, origin + DirFromAngle(endAngle) * radius);
        }

        private Vector3 DirFromAngle(float angleDeg)
        {
            float rad = (angleDeg + transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        }
    }
}
