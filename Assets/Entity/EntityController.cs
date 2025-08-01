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

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string Nation { get; set; } //move to EntityBody as Nation buffs can be added to hull
        public string AiName { get; set; }
        public string Type = "Sea";
        public string HullId;
        [SerializeField] public List<ScriptBase> ScriptList = new();
        //GER_e_mg45
        [SerializeField] public List<string> EquipmentIds = new();
        private EntityBody _entityBody;
        private IEntityController _controller;
        private void Awake()
        {
            _entityBody = GetComponent<EntityBody>();
            SetBodyType(Type);
        }
        private void SetBodyType(string type) => _entityBody.Type = type;
        private void Start()
        {
            if (IsPlayer)
            {
                PlayerController playerController = gameObject.AddComponent<PlayerController>();
                playerController.SetupKeyCodeDictionary();
                _controller = playerController;
                StartCoroutine(SetHull());
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
        public IEnumerator SetHull(string hullId = "")
        {
            if (_entityBody == null) yield break;

            HullId = string.IsNullOrEmpty(HullId) ? hullId : HullId;
            if (string.IsNullOrEmpty(HullId))
                GetDefaultHull();

            yield return StartCoroutine(LoadHullCoroutine());
            LoadEquipment();
        }

        private bool[] LoadEquipment()
        {
            bool[] installedEquips = new bool[EquipmentIds.Count];
            if (GameObjectsHandler.Objects.Count == 0) return installedEquips;
            for (int i = 0; i < EquipmentIds.Count; i++)
            {
                if (!GameObjectsHandler.Objects.ContainsKey(EquipmentIds[i])) continue;
            EquipmentContainer equipment = GameObjectsHandler.Objects[EquipmentIds[i]] as EquipmentContainer;
                installedEquips[i] = _entityBody.SetEquipment(equipment, i);
            }
            return installedEquips;
        }

        private void Update()
        {
            _controller?.UpdateControl(_entityBody);
        }
        private void GetDefaultHull()
        {
            switch (Type)
            {
                case "Sea":
                    HullId = "NONE_h_n_boat";
                    break;
            }
        }
        private IEnumerator LoadHullCoroutine()
        {
            if (GameObjectsHandler.Objects.Count == 0) yield break;
            HullContainer hull = GameObjectsHandler.Objects[HullId] as HullContainer;
            yield return StartCoroutine(_entityBody.StartSetupHullLayers(hull));
        }

        public void SetPointToMove(Transform target) => _controller.SetMovementPoint(target);
        public void SetTarget(Transform target) => _controller.SetTargetPoint(target);
    }
}