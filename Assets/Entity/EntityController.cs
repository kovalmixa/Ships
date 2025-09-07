using System.Collections.Generic;
using Assets.Entity.AI;
using Assets.Entity.DataContainers;
using Assets.Entity.Player;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using Assets.Entity.Interfaces;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string AiName { get; set; }
        public Dictionary<int, string> DefaultHullIds = new() //перенести в другое место: боты не могут снимать корпус
        {
            { 0, "NONE/boat" },
            { 1, "NONE/car" },
            { 2, "NONE/helicopter" }
        };
        [SerializeField] public List<ScriptBase> ScriptList = new();

        public EntityDataContainer Data = new();
        private IEntityController _controller;
        private EntityBody _entityBody;


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
            Queue<IScript> scripts = new();
            foreach (IScript scriptObj in ScriptList)
            {
                scripts.Enqueue(scriptObj);
            }
            aiController.ScriptList = scripts;

        }

        public void SetHull(string hullId = "")
        {
            if (Data == null) return;
            Data.HullId = string.IsNullOrEmpty(Data.HullId) ? hullId : Data.HullId;
            if (string.IsNullOrEmpty(Data.HullId))
                Data.HullId = DefaultHullIds[_entityBody.Type];
            _entityBody.SetHull(Data.HullId);
            LoadEquipment();
        }

        private bool[] LoadEquipment()
        {
            bool[] installedEquips = new bool[Data.EquipmentIds.Count];
            for (int i = 0; i < Data.EquipmentIds.Count; i++)
            {
                installedEquips[i] = _entityBody.SetEquipment(Data.EquipmentIds[i].Key, Data.EquipmentIds[i].Value);
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