using System.Collections.Generic;
using Assets.Entity.AI;
using Assets.Entity.Player;
using UnityEngine;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using System;
using System.Linq;
using Assets.DataContainers;
using Assets.Entity.Hull;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Scripts;
using Cinemachine;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        [SerializeField] private EntityHullSetup _entityHullSetup;
        public EntityDataContainer Data = new();
        private IEntityController _controller;
        public HullBase Hull;

        private void Awake()
        {
        }

        private void Start()
        {
            if (IsPlayer)
            {
                _controller = gameObject.AddComponent<PlayerController>();
            }
            var camera_node = SceneNodesHandler.GetNode("CameraNodes").transform.Find("Virtual Camera");
        }

        public void Setup(EntityDataContainer data)
        {
            if (data == null) return;
            Data.EquipmentIds = data.EquipmentIds;
            SetHull(data.HullId);
            var dPosition = data.Position;
            if (dPosition != Vector2.zero) transform.position = dPosition;
        }

        public bool SetHull(string hullId)
        {
            if (hullId == null) return false;
            HullBase newHullBase = _entityHullSetup.SetHull(hullId);
            newHullBase.Root = transform;
            Hull = newHullBase;
            Data.HullId = hullId;
            for (int i = 0; i < Data.EquipmentIds.Count; i++)
                if (!_entityHullSetup.SetEquipment(Data.EquipmentIds[i].Key, Data.EquipmentIds[i].Value))
                    Data.EquipmentIds.RemoveAt(i);

            return true;
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!_entityHullSetup.SetEquipment(equipmentId, index)) return false;
            Data.EquipmentIds.Add(new KeyValuePair<string, int>(equipmentId, index));
            return true;
        }

        public void SetupScripts(params ScriptBase[] scripts)
        {
            _controller = gameObject.AddComponent<AiController>();
            AiController aiController = _controller as AiController;
            Queue<ScriptBase> scriptsQueue = new(scripts);
            aiController.Scripts = scriptsQueue;
        }

        public void ActivateCommand(Vector3 position, string activationCommand)
        {
            if (activationCommand == "") return;
            if (TypeListHandler.IsWeaponEquipment(activationCommand)) if (IsAttackActionForbidden(position)) return;
            var activationTypes = TypeListHandler.TryGetSubType(activationCommand);
            foreach (var equipment in Hull.Equipments)
            {
                if (equipment.EquipmentContainer == null) continue;
                var type = equipment.EquipmentContainer.General.Class;
                if (activationTypes != null)
                {
                    if (activationTypes.Contains(type)) equipment.Activate(position);
                }
                else if (equipment.EquipmentContainer.General.Class == activationCommand || equipment.EquipmentContainer.Id == activationCommand)
                    equipment.Activate(position);
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

        public void SetPointToMove(Transform target) => _controller.SetMovementPoint(target);

        public void SetTarget(Transform target) => _controller.SetTargetPoint(target);

        private void Update()
        {
            if (Hull == null) return;
            _controller?.UpdateControl(this);
        }
    }
}