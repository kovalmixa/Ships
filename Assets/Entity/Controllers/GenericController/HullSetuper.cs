using System.Linq;
using Assets.Entity.Hull;
using Entity.Controllers.AI;
using UnityEngine;

namespace Entity.Controllers.GenericController
{
    public class EntityHullSetup : MonoBehaviour
    {
        private EntityController entityController;
        [SerializeField] private GameObject despawnPrefab;
        private void Awake()
        {
            entityController = GetComponent<EntityController>();
        }

        public HullBase SetHull(string hullId)
        {
            Transform bodyTrans;
            if (entityController.hull) bodyTrans = entityController.hull.transform;
            else bodyTrans = entityController.transform;
            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(hullId, bodyTrans.position, Quaternion.identity, bodyTrans);
            if (newHull == null) return null;
            var hull = entityController.hull;
            if (hull != null) Destroy(hull.gameObject);
            SetupNodes(newHull);
            return newHull.GetComponent<HullBase>();
        }

        private void SetupNodes(GameObject hull)
        {
            if (entityController.isPlayer)
            {
                CameraController.Instance.Follow(hull.transform);
            }
            else
            {
                GameObject despawn = Instantiate(despawnPrefab, hull.transform.position, hull.transform.rotation, hull.transform);
                despawn.GetComponent<Despawn>().SetEntity(entityController.gameObject);
            }
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (equipmentId == "") return false;
            var obj = PrefabLoader.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;
            var equipment = obj.GetComponentInChildren<Assets.Entity.Equipment.Equipment>();
            if (equipment == null) return false;
            equipment.EntityController = entityController;
            foreach (var equipmentAnchor in entityController.hull.EquipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                if (!equipmentAnchor.CanBePlaced(equipment, index)) continue;
                equipmentAnchor.SetTransform(equipment);
                entityController.hull.Equipments.Add(equipment);
                return true;
            }
            return false;
        }
    }
}
