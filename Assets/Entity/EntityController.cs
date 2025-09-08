using System.Collections.Generic;
using Assets.Entity.AI;
using Assets.Entity.Player;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using Assets.Entity.Interfaces;
using Assets.Entity.Equipment;
using Assets.Handlers;
using System;
using Assets.DataContainers;
using UnityEditor.SceneManagement;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string AiName { get; set; }
        [SerializeField] public List<ScriptBase> ScriptList = new();

        private EntityBodySetup _entityBodySetup;
        private Activator _activator;

        public EntityDataContainer Data = new();
        private IEntityController _controller;
        public Hull.Hull Hull;
        public ActivationContainer[] Activations;

        private void Awake()
        {
            _activator = gameObject.AddComponent<Activator>();
        }
        private void Start()
        {

            if (IsPlayer)
            {
                _controller = gameObject.AddComponent<PlayerController>();
            }
            else
            {
                SetupScripts();
                ScriptList.Clear();
            }
        }

        public void Setup(EntityDataContainer data)
        {
            if (data == null) return;
            if (_entityBodySetup == null) _entityBodySetup = gameObject.AddComponent<EntityBodySetup>();
            Data.EquipmentIds = data.EquipmentIds;
            SetHull(data.HullId);
            var dPosition = data.Position;
            if (dPosition != Vector2.zero) transform.position = dPosition;
        }

        public bool SetHull(string hullId)
        {
            if (hullId == null) return false;
            Hull.Hull newHull = _entityBodySetup.SetHull(hullId);
            if (newHull == null) return false;
            Hull = newHull;
            Data.HullId = hullId;
            for (int i = 0; i < Data.EquipmentIds.Count; i++)
                if (!_entityBodySetup.SetEquipment(Data.EquipmentIds[i].Key, Data.EquipmentIds[i].Value))
                    Data.EquipmentIds.RemoveAt(i);
            return true;
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!_entityBodySetup.SetEquipment(equipmentId, index)) return false;
            Data.EquipmentIds.Add(new KeyValuePair<string, int>(equipmentId, index));
            return true;
        }

        public void SetupScripts()
        {
            _controller = gameObject.AddComponent<AiController>();
            AiController aiController = _controller as AiController;
            Queue<IScript> scripts = new();
            foreach (IScript scriptObj in ScriptList)
            {
                scripts.Enqueue(scriptObj);
            }
            aiController.ScriptList = scripts;
        }

        public void ActivateCommand(Vector3 position, string activationCommand)
        {
            if (activationCommand == "") return;
            //заменить эту дебильную атаку на атаку снарядами, авиацией и торпеды/ракеты
            if (!ActivationHandler.IsPassive(activationCommand))
                if (IsAttackActionForbidden(position)) return;
            if (Activations is { Length: > 0 })
            {
                foreach (var activation in Activations)
                {
                    if (activation.Type == activationCommand) Activate(position, activationCommand);
                }
            }
            foreach (var equipmentGameObject in Hull.Equipments)
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

        private bool IsAttackActionForbidden(Vector3 position)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position)) return true;
            foreach (var equipment in Hull.Equipments)
            {
                col = equipment.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(position)) return true;
            }
            return false;
        }

        public void Activate(Vector3 targetPosition, string type = null) => _activator.TryActivate(targetPosition, type);

        public void SetPointToMove(Transform target) => _controller.SetMovementPoint(target);

        public void SetTarget(Transform target) => _controller.SetTargetPoint(target);

        private void Update()
        {
            if (Hull == null) return;
            _controller?.UpdateControl(this);
        }
        void LateUpdate()
        {
            if (Hull != null)
            {
                transform.position = Hull.transform.position;
                transform.rotation = Hull.transform.rotation;
            }
        }

    }
}