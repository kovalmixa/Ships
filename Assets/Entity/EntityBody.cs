using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Entity.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using UnityEngine;

namespace Assets.Entity
{
    public class EntityBody : InGameObject, IActivation, IDamageable
    {
        public int Type
        {
            get
            {
                if (EntityContainer.HullContainer.General == null) return 0;
                return 0;
            }
            set{}
        }
        private Activator _activator;
        public EntityController EntityController;
        public EntityContainer EntityContainer = new();
        private List<GameObject> _equipments = new();
        public ActivationContainer[] Activations
        {
            get => EntityContainer.HullContainer.OnActivate; 
            set => EntityContainer.HullContainer.OnActivate = value;
        }
        public float MaxSpeed = 5f;
        public float Acceleration = 3f;
        public float RotationSpeed = 60f;
        private float _targetSpeed;

        public float SpeedLevel
        {
            get => Speed;
            set => Speed = value;
        }

        private int _maxSpeedLevel = 3;
        public int MaxSpeedLevel => _maxSpeedLevel;
        private int _minSpeedLevel = -1;
        public int MinSpeedLevel => _minSpeedLevel;

        private void Awake()
        {
            _activator = gameObject.AddComponent<Activator>();
        }

        public void SetHull(string hullId)
        {
            GameObject newBody = GameObjectsHandler.Instance.InstantiatePrefab(hullId,transform.position, Quaternion.identity, transform);
            if (newBody == null) return;
            Destroy(Body);
            Body = newBody;
            EntityContainer.HullContainer = Body.GetComponent<Hull>().HullContainer;
            _equipments.Clear();
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (equipmentId == "") return false;
            var obj = GameObjectsHandler.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;
            var equipment = obj.GetComponent<Equipment.Equipment>();
            if (equipment == null) return false;
            foreach (var equipmentAnchor in Body.GetComponent<Hull>().EquipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                equipmentAnchor.SetTransform(equipment);
                _equipments.Add(equipment.gameObject);
                return true;
            }
            return false;
        }

        public void Movement(float rotationDirection)
        {
            switch (Type)
            {
                case 0:
                {
                    _targetSpeed = SpeedLevel * (MaxSpeed / _maxSpeedLevel);
                    CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, _targetSpeed, Acceleration * Time.deltaTime);
                    transform.Rotate(Vector3.forward, -rotationDirection * RotationSpeed * Time.deltaTime);
                    transform.Translate(Vector3.up * CurrentSpeed * Time.deltaTime, Space.Self);
                    break;
                }
            }
        }

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in _equipments)
            {
                eq.GetComponent<Equipment.Equipment>().Rotate(target);
            }
        }

        public void ActivateCommand(Vector3 position, string activationCommand)
        {
            if (activationCommand == "") return;
            //заменить эту дебильную атаку на атаку снарядами, авиацией и торпеды/ракеты
            if (!ActivationHandler.IsPassive(activationCommand))
                if (IsAttackActionForbidden(position)) return;
            if (Activations != null && Activations.Length > 0)
            {
                foreach (var activation in Activations)
                {
                    if (activation.Type == activationCommand) Activate(position, activationCommand);
                }
            }
            foreach (var equipmentGameObject in _equipments)
            {
                Equipment.Equipment equipment = equipmentGameObject.GetComponent<Equipment.Equipment>();
                if (equipment.EquipmentContainer == null) continue;
                string type = equipment.EquipmentContainer.General.Class;
                //заменить эту дебильную атаку на атаку снарядами, авиацией и торпеды/ракеты
                if (activationCommand == "Attack")
                {
                    if (TypeListHandler.IsWeapon(type)) equipment.Activate(position);
                    continue;
                }
                if (equipment.EquipmentContainer.General.Class == activationCommand ||
                    equipment.EquipmentContainer.Id == activationCommand) equipment.Activate(position);
            }
        }

        public void Activate(Vector3 targetPosition, string type = null) => _activator.TryActivate(targetPosition,  type);

        private bool IsAttackActionForbidden(Vector3 position)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position)) return true;
            foreach (var equipment in _equipments)
            {
                col = equipment.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(position)) return true;
            }
            return false;
        }

        public void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }
    }
}
