using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using UnityEngine;

namespace Assets.Entity
{
    public class EntityBodySetup : MonoBehaviour
    {

        private EntityController _entityController;

        private void Awake()
        {
            _entityController = GetComponent<EntityController>();
        }
        public Hull.Hull SetHull(string hullId)
        {
            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(hullId, transform.position, Quaternion.identity, transform);
            if (newHull == null) return null;
            var hull = _entityController.Hull;
            if (hull != null) Destroy(hull.gameObject);
            return newHull.GetComponent<Hull.Hull>();
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
