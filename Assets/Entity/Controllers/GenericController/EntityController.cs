using System.Collections.Generic;
using System.Linq;
using Assets.Entity;
using Assets.Entity.Hull;
using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.AI;
using Scripts;
using UnityEngine;

namespace Entity.Controllers.GenericController
{
    public class EntityController : MonoBehaviour
    {
        public bool isPlayer;
        [SerializeField] private EntityHullSetup _entityHullSetup;
        public EntityDataContainer data;
        private IEntityController _controller;
        public HullBase hull;

        private void Start()
        {
            if (isPlayer)
            {
                _controller = gameObject.AddComponent<PlayerController>();
            }
            var cameraNode = SceneNodesHandler.GetNode("CameraNodes").transform.Find("Virtual Camera");
        }

        public void Setup(EntityDataContainer data)
        {
            if (data == null) return;
            this.data.equipmentIds = data.equipmentIds;
            SetHull(data.hullId);
            var dPosition = data.position;
            if (dPosition != Vector2.zero) transform.position = dPosition;
        }

        public bool SetHull(string hullId)
        {
            if (hullId == null) return false;
            HullBase newHullBase = _entityHullSetup.SetHullNodeLogic(hullId);
            newHullBase.root = transform;
            hull = newHullBase;
            data.hullId = hullId;
            for (int i = 0; i < data.equipmentIds.Count; i++)
                if (!_entityHullSetup.SetEquipmentNodeLogic(data.equipmentIds[i].Key, data.equipmentIds[i].Value))
                    data.equipmentIds.RemoveAt(i);

            return true;
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!_entityHullSetup.SetEquipmentNodeLogic(equipmentId, index)) return false;
            data.equipmentIds.Add(new KeyValuePair<string, int>(equipmentId, index));
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
            var activationTypes = TypeListHandler.TryGetEquipSubTypes(activationCommand);
            foreach (var equipment in hull.equipments)
            {
                if (equipment.equipmentContainer == null) continue;
                var type = equipment.equipmentContainer.general.Class;
                if (activationTypes != null)
                {
                    if (activationTypes.Contains(type)) equipment.Activate(position);
                }
                else if (equipment.equipmentContainer.general.Class == activationCommand || equipment.equipmentContainer.Id == activationCommand)
                    equipment.Activate(position);
            }
        }

        private bool IsAttackActionForbidden(Vector3 position)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position)) return true;
            foreach (var equipment in hull.equipments)
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
            if (hull == null) return;
            _controller?.UpdateControl(this);
        }
    }
}