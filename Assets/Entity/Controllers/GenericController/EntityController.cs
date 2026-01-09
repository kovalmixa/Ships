using System.Collections.Generic;
using System.Linq;
using Assets.DataContainers;
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
        [SerializeField] private EntityHullSetup entityHullSetup;
        public EntityDataContainer data = new();
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
            this.data.EquipmentIds = data.EquipmentIds;
            SetHull(data.HullId);
            var dPosition = data.Position;
            if (dPosition != Vector2.zero) transform.position = dPosition;
        }

        public bool SetHull(string hullId)
        {
            if (hullId == null) return false;
            HullBase newHullBase = entityHullSetup.SetHull(hullId);
            newHullBase.Root = transform;
            hull = newHullBase;
            data.HullId = hullId;
            for (int i = 0; i < data.EquipmentIds.Count; i++)
                if (!entityHullSetup.SetEquipment(data.EquipmentIds[i].Key, data.EquipmentIds[i].Value))
                    data.EquipmentIds.RemoveAt(i);

            return true;
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!entityHullSetup.SetEquipment(equipmentId, index)) return false;
            data.EquipmentIds.Add(new KeyValuePair<string, int>(equipmentId, index));
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
            foreach (var equipment in hull.Equipments)
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
            foreach (var equipment in hull.Equipments)
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