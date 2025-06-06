using System;
using System.Collections.Generic;
using System.IO;
using Assets.Entity.AI;
using Assets.Entity.DataContainers;
using Assets.Entity.Player;
using Assets.InGameMarkers.Scripts;
using Assets.Handlers;
using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public bool IsPlayer = false;
        public string Nation { get; set; } //move to Entity as Nation buffs can be added to hull
        public string AiName { get; set; }
        public string Type = "ship";
        public string HullId;
        [SerializeField] public List<ScriptBase> ScriptList = new();
        [SerializeField] public List<string> WeaponIds = new();
        private Entity _entity;
        private IEntityController _controller;
        private void Start()
        {
            if (IsPlayer)
            {
                _controller = gameObject.AddComponent<PlayerController>();
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
            aiController.Size = _entity.Size;
            Queue<IScript> scripts = new();
            foreach (IScript scriptObj in ScriptList)
            {
                scripts.Enqueue(scriptObj);
            }
            aiController.ScriptList = scripts;

        }
        public void SetHull(string hullId = "")
        {
            HullId = hullId;
            _entity = GetComponent<Entity>();
            if (HullId == "")
                GetDefaultHull();
            LoadHull();
        }
        private void Update()
        {
            _controller?.UpdateControl(_entity);
        }
        private void GetDefaultHull()
        {
            switch (Type)
            {
                case "ship":
                    HullId = "NONE_h_n_boat";
                    break;
            }
        }
        private void LoadHull()
        {
            if (ObjectPoolHandler.Objects.Count == 0) return;
            HullContainer hull = ObjectPoolHandler.Objects[HullId] as HullContainer;
            _entity.SetupHullLayers(hull);
        }
        public void SetPointToMove(Transform target) => _controller.SetMovementPoint(target);
        public void SetTarget(Transform target) => _controller.SetTargetPoint(target);
    }
}