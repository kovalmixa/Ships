using System.Collections.Generic;
using Assets.Entity.AI;
using Assets.Entity.DataContainers;
using Assets.Entity.Player;
using Assets.InGameMarkers.Scripts;
using Assets.Handlers;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using Assets.Entity.Interfaces;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string Nation { get; set; } //move to EntityBody as Nation buffs can be added to hull
        public string AiName { get; set; }
        public string HullId;
        public Dictionary<int, string> DefaultHullIds = new() //перенести в другое место: боты не могут снимать корпус
        {
            { 0, "NONE/boat" },
            { 1, "NONE/car" },
            { 2, "NONE/helicopter" }
        };
        [SerializeField] public List<ScriptBase> ScriptList = new();
        //GER_e_mg45
        [SerializeField] public List<string> EquipmentIds = new();
        private EntityBody _entityBody;
        private IEntityController _controller;

        private void Awake()
        {
            _entityBody = GetComponent<EntityBody>();
        }

        private void Start()
        {
            if (IsPlayer)
            {
                PlayerController playerController = gameObject.AddComponent<PlayerController>();
                playerController.SetupKeyCodeDictionary();
                _controller = playerController;
                SetHull();
            }
            else
            {
                SetupScripts();
                ScriptList.Clear();
            }
        }

        public void SetupScripts()
        {
            _controller = gameObject.AddComponent<AiController>();
            AiController aiController = _controller as AiController;
            aiController.Size = _entityBody.CollisionSize;
            Queue<IScript> scripts = new();
            foreach (IScript scriptObj in ScriptList)
            {
                scripts.Enqueue(scriptObj);
            }
            aiController.ScriptList = scripts;

        }

        public void SetHull(string hullId = "")
        {
            if (_entityBody == null) return;
            HullId = string.IsNullOrEmpty(HullId) ? hullId : HullId;
            if (string.IsNullOrEmpty(HullId))
                HullId = DefaultHullIds[_entityBody.Type];
            _entityBody.SetHull(HullId);
            LoadEquipment();
        }

        private bool[] LoadEquipment()
        {
            bool[] installedEquips = new bool[EquipmentIds.Count];
            for (int i = 0; i < EquipmentIds.Count; i++)
            {
                installedEquips[i] = _entityBody.SetEquipment(EquipmentIds[i], i);
            }
            return installedEquips;
        }

        public void SetPointToMove(Transform target) => _controller.SetMovementPoint(target);

        public void SetTarget(Transform target) => _controller.SetTargetPoint(target);

        private void Update()
        {
            _controller?.UpdateControl(_entityBody);
        }
    }
}