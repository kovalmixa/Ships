using Assets.Entity.Hull;
using Assets.Handlers.FileHandlers;
using Entity.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public sealed class EntityAssembler
    {
        private readonly global::Entity.Controllers.EntityController _entity;

        public EntityAssembler(global::Entity.Controllers.EntityController entity) => _entity = entity;
        public event Action<HullBase> onSetHull;
        public event Action<Equipment.Equipment> onSetEquipment;

        public async Task<bool> Build(EntityData data)
        {
            if (data == null) return false;

            _entity.data = data;
            if (! await SetHull(data.hullId)) return false;

            foreach (var equipment in data.equipmentIds.ToList())
            {
                bool isSuccess = false;
                while (await AddEquipment(equipment.Key, equipment.Value)) isSuccess = true;
                if (!isSuccess) data.equipmentIds.Remove(equipment); 
                //code for placing it to inventory
            }
            if (data.position != Vector2.zero) _entity.transform.position = data.position;

            return true;
        }

        public async Task<bool> SetHull(string hullId)
        {
            if (string.IsNullOrEmpty(hullId)) return false;

            if (_entity.hull != null) UnityEngine.Object.Destroy(_entity.hull.gameObject);
            var hullObj = await PrefabLoader.Instance.InstantiatePrefabAsync(
                hullId,
                _entity.transform.position,
                Quaternion.identity,
                _entity.transform);

            if (hullObj == null) return false;
            var hull = hullObj.GetComponent<HullBase>();
            if (hull == null) return false;
            hull.root = _entity.transform;
            _entity.hull = hull;
            hull.Setup(_entity);
            onSetHull?.Invoke(hull);
            return true;
        }

        public async Task<bool> AddEquipment(string equipmentId, int index)
        {
            if (_entity.hull == null) return false;

            var obj = await PrefabLoader.Instance.InstantiatePrefabAsync(
                equipmentId,
                Vector3.zero,
                Quaternion.identity);

            if (obj == null) return false;

            var equipment = obj.GetComponentInChildren<Equipment.Equipment>();
            if (equipment == null)
            {
                UnityEngine.Object.Destroy(obj);
                return false;
            }

            foreach (var anchor in _entity.hull.equipmentAnchors)
            {
                if (!anchor.CanBePlaced(equipment, index)) continue;

                anchor.Place(equipment);
                equipment.Setup(_entity);
                onSetEquipment?.Invoke(equipment);
                _entity.hull.equipments.Add(equipment);

                return true;
            }

            UnityEngine.Object.Destroy(obj);
            return false;
        }
    }
}