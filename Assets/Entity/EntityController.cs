using System.Collections.Generic;
using Assets.Entity.AI;
using Assets.Entity.Player;
using UnityEngine;
using Assets.Entity.Interfaces;
using Assets.Entity.Equipment;
using Assets.Handlers;
using System;
using System.Linq;
using Assets.DataContainers;
using Assets.Scripts.Actions;
using Assets.Scripts.Scripts;
using UnityEditor.SceneManagement;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string AiName { get; set; }
        [SerializeField] public List<ScriptBase> ScriptList = new();

        private EntityHullSetup _entityBodySetup;

        public EntityDataContainer Data = new();
        private IEntityController _controller;
        public Hull.Hull Hull;

        private void Awake()
        {
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
            if (_entityBodySetup == null) _entityBodySetup = gameObject.AddComponent<EntityHullSetup>();
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