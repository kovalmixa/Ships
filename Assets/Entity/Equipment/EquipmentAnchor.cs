using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    [ExecuteInEditMode]
    public class EquipmentAnchor : MonoBehaviour
    {
        public int index;

        public EquipmentType equipmentType = EquipmentType.None;
        public ProjectileType projectileType = ProjectileType.None;

        public SizeType sizeType;

        public Vector2 rotationSector;
        public Vector2[] activationSectors;
        public int orderLayer;

        private bool _isPlaced = false;

        public bool CanBePlaced(Equipment equipment, int index)
        {
            if (this.index != index || _isPlaced) return false;

            EquipmentContainer equipmentContainer = equipment.Data;
            if (equipmentContainer == null || sizeType != equipmentContainer.general.SizeType) return false;
            bool equipmentMatch = equipmentType == EquipmentType.None || equipmentType == equipmentContainer.Type;
            bool projectileMatch = projectileType == ProjectileType.None || projectileType == equipmentContainer.ProjectileType;
            return equipmentMatch && projectileMatch;
        }

        public void Place(Equipment equipment)
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
            _isPlaced = true;
        }

        #region Editor

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

        #endregion
    }
}