using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    [ExecuteInEditMode]
    public class EquipmentAnchor : MonoBehaviour
    {
        public enum AnchorFilterMode { ByMasterType, BySubType }

        public int index;

        public AnchorFilterMode filterMode;
        public EquipmentMasterType masterClassType;
        public EquipmentSubType subClassType;

        public SizeType sizeType;

        public Vector2 rotationSector;
        public Vector2[] activationSectors;
        public int orderLayer;
        public const bool isStatic = false;

        public bool CanBePlaced(Equipment equipment, int index)
        {
            if (isStatic) return false;
            if (this.index != index) return false;

            EquipmentContainer equipmentContainer = equipment.equipmentContainer;
            if (sizeType != equipmentContainer.general.sizeType) return false;
            EquipmentSubType eqSubType = equipmentContainer.type;
            if (filterMode == AnchorFilterMode.BySubType) return eqSubType == subClassType;
            else
            {
                EquipmentMasterType eqMasterType = EquipmentHandler.TryGetMasterType(eqSubType);
                return eqMasterType == masterClassType;
            }
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
            GameObjectHandler.SetRenderLayerOrder(gameObject, orderLayer);
        }

        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;
            Gizmos.color = Color.yellow;
            DrawSector(origin, rotationSector, 2f);
            if (activationSectors != null)
            {
                Gizmos.color = Color.red;
                foreach (var sector in activationSectors)
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