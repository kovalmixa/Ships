using System.Linq;
using Assets.Entity.Hull;
using UnityEngine;

namespace Assets.Entity
{
    public class EntityHullSetup : MonoBehaviour
    {

        private EntityController _entityController;
        [SerializeField] private Transform _entityBody;
        private void Awake()
        {
            _entityController = GetComponent<EntityController>();
        }

        public HullBase SetHull(string hullId)
        {
            Transform bodyTrans;
            if (_entityController.Hull) bodyTrans = _entityController.Hull.transform;
            else bodyTrans = _entityBody;
            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(hullId, bodyTrans.position, Quaternion.identity, bodyTrans);
            if (newHull == null) return null;
            var hull = _entityController.Hull;
            if (hull != null) Destroy(hull.gameObject);
            return newHull.GetComponent<HullBase>();
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (equipmentId == "") return false;
            var obj = PrefabLoader.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;
            var equipment = obj.GetComponentInChildren<Equipment.Equipment>();
            if (equipment == null) return false;
            equipment.EntityController = _entityController;
            foreach (var equipmentAnchor in _entityController.Hull.EquipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                if (!equipmentAnchor.CanBePlaced(equipment, index)) continue;
                equipmentAnchor.SetTransform(equipment);
                _entityController.Hull.Equipments.Add(equipment);
                return true;
            }
            return false;
        }
    }
}
