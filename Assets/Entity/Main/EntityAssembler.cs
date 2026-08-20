using Assets.Entity.Hull;
using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public sealed class EntityAssembler
    {
        private readonly EntityController _entity;

        public EntityAssembler(EntityController entity) => _entity = entity;
        public event Action<HullBase> onSetHull;
        public event Action<Equipment.Equipment> onSetEquipment;

        public async Task<bool> Build(EntityData data)
        {
            if (data == null) return false;

            _entity.data = data;
            if (!await SetHull(data.hullId)) return false;

            for (int i = data.equipmentSlots.Count - 1; i >= 0; i--)
            {
                var slot = data.equipmentSlots[i];
                bool isSuccess = false;

                var equipmentObj = await PrefabLoader.Instance.InstantiatePrefabAsync(
                    slot.equipmentId,
                    Vector3.zero,
                    Quaternion.identity);
                if (equipmentObj == null) data.equipmentSlots.Remove(slot);
                else
                {
                    while (AddEquipment(equipmentObj, slot.number)) isSuccess = true;
                    if (!isSuccess)
                    {
                        data.equipmentSlots.Remove(slot);
                        // code for placing it to inventory if unsuccessful
                    }
                    GameObject.DestroyImmediate(equipmentObj);
                }
            }
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

        public bool AddEquipment(GameObject eqObj, int index)
        {
            if (eqObj == null) return false;
            var clone = GameObjectHandler.Clone(eqObj);

            var equipment = clone.GetComponentInChildren<Equipment.Equipment>();
            if (equipment == null)
            {
                UnityEngine.Object.Destroy(clone);
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

            UnityEngine.Object.Destroy(clone);
            return false;
        }
    }
}