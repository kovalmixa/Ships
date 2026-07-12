using Assets.Entity.Hull;
using Entity.Controllers;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public sealed class EntityAssembler
    {
        private readonly EntityController _entity;

        public EntityAssembler(EntityController entity) => _entity = entity;

        public bool Build(EntityDataContainer data)
        {
            if (data == null) return false;

            _entity.data = data;

            if (!SetHull(data.hullId)) return false;

            foreach (var equipment in data.equipmentIds.ToList())
                if (!AddEquipment(equipment.Key, equipment.Value))
                    data.equipmentIds.Remove(equipment);

            if (data.position != Vector2.zero)
                _entity.transform.position = data.position;

            return true;
        }

        public bool SetHull(string hullId)
        {
            if (string.IsNullOrEmpty(hullId)) return false;

            if (_entity.hull != null) Object.Destroy(_entity.hull.gameObject);

            var hullObj = PrefabLoader.Instance.InstantiatePrefab(
                hullId,
                _entity.transform.position,
                Quaternion.identity,
                _entity.transform);

            if (hullObj == null) return false;

            var hull = hullObj.GetComponent<HullBase>();

            if (hull == null) return false;

            hull.root = _entity.transform;
            _entity.hull = hull;
            _entity.Buffs.Register(hull.BuffController);

            return true;
        }

        public bool AddEquipment(string equipmentId, int index)
        {
            if (_entity.hull == null) return false;

            var obj = PrefabLoader.Instance.InstantiatePrefab(
                equipmentId,
                Vector3.zero,
                Quaternion.identity);

            if (obj == null) return false;

            var equipment = obj.GetComponentInChildren<Equipment.Equipment>();

            if (equipment == null)
            {
                Object.Destroy(obj);
                return false;
            }

            equipment.entityController = _entity;

            foreach (var anchor in _entity.hull.equipmentAnchors)
            {
                if (!anchor.CanBePlaced(equipment, index)) continue;

                anchor.SetTransform(equipment);
                _entity.hull.equipments.Add(equipment);
                _entity.Buffs.Register(equipment.BuffController);
                _entity.Abilities.Register(equipment);
                return true;
            }

            Object.Destroy(obj);
            return false;
        }
    }
}