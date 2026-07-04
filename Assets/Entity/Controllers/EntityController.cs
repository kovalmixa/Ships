using System.Collections.Generic;
using System.Linq;
using Assets.Common;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.AI;
using Scripts;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject
    {
        public EntityDataContainer data;
        public AbbilitiesController abbilitiesController;
        private IEntityController _controller;
        [SerializeField] private GameObject _despawnPrefab;
        public string Id { get; set; }
        public HullBase hull;

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            if (GameObjectHandler.GetAI(this) == null) GameObjectHandler.RegisterPlayer(this);
        }

        public void SetController(IEntityController controller)
        {
            _controller = controller;
        }

        private HullBase SetHullNodeLogic(string hullId)
        {
            Transform bodyTrans;
            if (hull) bodyTrans = hull.transform;
            else bodyTrans = transform;
            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(
                hullId, bodyTrans.position, Quaternion.identity, bodyTrans);

            if (newHull == null) return null;
            if (hull != null) Destroy(hull.gameObject);

            SetupNodes(newHull);
            return newHull.GetComponent<HullBase>();
        }

        private void SetupNodes(GameObject hull)
        {
            if (GameObjectHandler.IsPlayer(this))
            {
                CameraController.Instance.Follow(hull.transform);
            }
            else
            {
                GameObject despawn = Object.Instantiate(_despawnPrefab,
                    hull.transform.position, hull.transform.rotation, hull.transform);
                despawn.GetComponent<Despawn>().SetEntity(gameObject);
            }
        }

        private bool SetEquipmentNodeLogic(string equipmentId, int index)
        {
            if (equipmentId == "") return false;
            var obj = PrefabLoader.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;
            var equipment = obj.GetComponentInChildren<Assets.Entity.Equipment.Equipment>();
            if (equipment == null) return false;
            equipment.entityController = this;
            foreach (var equipmentAnchor in hull.equipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                //make spawn to inventory
                if (!equipmentAnchor.CanBePlaced(equipment, index)) continue;
                equipmentAnchor.SetTransform(equipment);
                hull.equipments.Add(equipment);
                abbilitiesController?.MarkDirty();
                return true;
            }
            return false;
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
            HullBase newHullBase = SetHullNodeLogic(hullId);
            newHullBase.root = transform;
            hull = newHullBase;
            data.hullId = hullId;
            for (int i = 0; i < data.equipmentIds.Count; i++)
                if (!SetEquipmentNodeLogic(data.equipmentIds[i].Key, data.equipmentIds[i].Value))
                    data.equipmentIds.RemoveAt(i);

            abbilitiesController?.MarkDirty();
            return true;
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!SetEquipmentNodeLogic(equipmentId, index)) return false;
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

        #endregion

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        private void Update()
        {
            if (hull == null) return;
            _controller?.UpdateControl(this);
        }
    }
}
