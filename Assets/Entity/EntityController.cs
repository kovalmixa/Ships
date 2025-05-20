using System.Collections.Generic;
using System.IO;
using Assets.Entity.AI;
using Assets.Entity.DataContainers;
using Assets.Entity.Player;
using Assets.Handlers;
using UnityEngine;

namespace Assets.Entity
{
    public class EntityController : MonoBehaviour
    {
        public string AssetObjectPath;
        public bool IsPlayer = false;
        public string Nation { get; set; }
        public string AiName { get; set; }
        public string Type = "ship";
        public string HullId;
        [SerializeField] public List<GameObject> RouteScriptsList;
        [SerializeField] public List<GameObject> ScriptAreaList;
        [SerializeField] public List<GameObject> ScriptList; //реализовать как скрипты
        [SerializeField] public List<string> WeaponIds = new();
        private Entity _entity;
        private IEntityController _controller;
        private void Start()
        {

            if (IsPlayer)
            {
                _controller = gameObject.AddComponent<PlayerController>();
            }
            else
            {
                _controller = gameObject.AddComponent<AiController>();
                AiController aiController = _controller as AiController;
                if (aiController != null) aiController.SetupRouteScripts(RouteScriptsList);
            }
            SetHull();

        }

        public void SetHull(string hullId = "")
        {
            HullId = hullId;
            _entity = GetComponent<Entity>();
            //if (HullId == "")
            //    GetDefaultHull();
            //LoadHull();
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
                    HullId = "NONE_h_sh_boat";
                    break;
            }
        }
        private void LoadHull()
        {
            string path = DataHandler.GetPathById(HullId, AssetObjectPath);
            int index = HullId.LastIndexOf('_');
            string name = HullId.Substring(index + 1);
            string jsonFilePath = path + '/' + name + ".json";

            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError($"File not found: {jsonFilePath}");
                return;
            }

            HullContainer hullData = DataHandler.LoadFromJson<HullContainer>(jsonFilePath);
            _entity.EntityData.HullData = hullData;
        }
    }
}