using System.Linq;
using Assets.Entity.Hull;
using Entity.Controllers.AI;
using UnityEngine;

namespace Entity.Controllers.GenericController
{
    public class EntityHullSetup : MonoBehaviour
    {
        private EntityController _entityController;
        [SerializeField] private GameObject _despawnPrefab;
        private void Awake()
        {
            _entityController = GetComponent<EntityController>();
        }

        public HullBase SetHull(string hullId)
        {
            Transform bodyTrans;
            if (_entityController.hull) bodyTrans = _entityController.hull.transform;
            else bodyTrans = _entityController.transform;
            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(hullId, bodyTrans.position, Quaternion.identity, bodyTrans);
            if (newHull == null) return null;
            var hull = _entityController.hull;
            if (hull != null) Destroy(hull.gameObject);
            SetupNodes(newHull);
            return newHull.GetComponent<HullBase>();
        }

        private void SetupNodes(GameObject hull)
        {
            if (_entityController.isPlayer)
            {
                CameraController.Instance.Follow(hull.transform);
            }
            else
            {
                GameObject despawn = Instantiate(_despawnPrefab, hull.transform.position, hull.transform.rotation, hull.transform);
                despawn.GetComponent<Despawn>().SetEntity(_entityController.gameObject);
            }
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (equipmentId == "") return false;
            var obj = PrefabLoader.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;
            var equipment = obj.GetComponentInChildren<Assets.Entity.Equipment.Equipment>();
            if (equipment == null) return false;
            equipment.EntityController = _entityController;
            foreach (var equipmentAnchor in _entityController.hull.EquipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                if (!equipmentAnchor.CanBePlaced(equipment, index)) continue;
                equipmentAnchor.SetTransform(equipment);
                _entityController.hull.Equipments.Add(equipment);
                return true;
            }
            return false;
        }
    }
}
