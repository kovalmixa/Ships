using System.Collections.Generic;
using System.Linq;
using Assets.Entity;
using Assets.Entity.Hull;
using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.AI;
using Scripts;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour
    {
        public EntityDataContainer data;
        private IEntityController _controller;
        [SerializeField] private GameObject _despawnPrefab;
        public HullBase hull;

        private void Start()
        {
            if (GameObjectHandler.GetAI(this) == null) GameObjectHandler.RegisterPlayer(this);
        }

        #region Setup

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
            if (hull != null) Object.Destroy(hull.gameObject);

            SetupNodes(newHull);
            return newHull.GetComponent<HullBase>();
        }

        private void SetupNodes(GameObject hull)
        {
            if (isPlayer)
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

        private void Update()
        {
            if (hull == null) return;
            _controller?.UpdateControl(this);
        }
    }
}