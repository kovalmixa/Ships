using Actions;
using Assets.Common;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.AI;
using Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility
    {
        public EntityDataContainer data;
        public AbbilitiesController abbilitiesController;
        private IDriver _driver;
        [SerializeField] private GameObject _despawnPrefab;
        public string Id { get; set; }
        public HullBase hull;

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            if (GameObjectHandler.GetAI(this) == null) GameObjectHandler.RegisterPlayer(this);
        }

        public void SetController(IDriver controller)
        {
            _driver = controller;
        }

        public void Setup(EntityDataContainer data)
        {
            if (data == null) return;

            this.data.equipmentIds = data.equipmentIds;
            SetHull(data.hullId);

            var dPosition = data.position;
            if (dPosition != Vector2.zero)
                transform.position = dPosition;
        }

        public bool SetHull(string hullId)
        {
            if (hullId == null) return false;
            if (hull != null)
            {
                Destroy(hull.gameObject);
                hull = null;
            }

            HullBase newHullBase = SetHullNodeLogic(hullId);
            if (newHullBase == null) return false;

            newHullBase.root = transform;
            hull = newHullBase;
            data.hullId = hullId;

            for (int i = data.equipmentIds.Count - 1; i >= 0; i--)
            {
                var pair = data.equipmentIds[i];
                if (!SetEquipmentNodeLogic(pair.Key, pair.Value)) data.equipmentIds.RemoveAt(i);
            }

            abbilitiesController?.MarkDirty();
            return true;
        }

        private HullBase SetHullNodeLogic(string hullId)
        {
            Transform bodyTrans = hull != null ? hull.transform : transform;

            GameObject newHull = PrefabLoader.Instance.InstantiatePrefab(
                hullId, bodyTrans.position, Quaternion.identity, bodyTrans);

            if (newHull == null) return null;

            SetupNodes(newHull);
            return newHull.GetComponent<HullBase>();
        }

        private void SetupNodes(GameObject hullObject)
        {
            if (GameObjectHandler.IsPlayer(this))
            {
                CameraController.Instance.Follow(hullObject.transform);
            }
            else
            {
                GameObject despawn = Object.Instantiate(_despawnPrefab,
                    hullObject.transform.position, hullObject.transform.rotation, hullObject.transform);
                if (despawn.TryGetComponent<Despawn>(out var despawnComp)) despawnComp.SetEntity(gameObject);
            }
        }

        public bool SetEquipment(string equipmentId, int index)
        {
            if (!SetEquipmentNodeLogic(equipmentId, index)) return false;
            data.equipmentIds.Add(new KeyValuePair<string, int>(equipmentId, index));
            return true;
        }

        private bool SetEquipmentNodeLogic(string equipmentId, int index)
        {
            if (string.IsNullOrEmpty(equipmentId)) return false;
            if (hull == null) return false;

            var obj = PrefabLoader.Instance.InstantiatePrefab(equipmentId, Vector3.zero, Quaternion.identity);
            if (obj == null) return false;

            var equipment = obj.GetComponentInChildren<Assets.Entity.Equipment.Equipment>();
            if (equipment == null)
            {
                Destroy(obj);
                return false;
            }

            equipment.entityController = this;

            foreach (var equipmentAnchor in hull.equipmentAnchors.Where(go => go.transform.childCount == 0))
            {
                if (!equipmentAnchor.CanBePlaced(equipment, index)) continue;

                equipmentAnchor.SetTransform(equipment);
                hull.equipments.Add(equipment);
                abbilitiesController?.MarkDirty();
                return true;
            }

            Destroy(obj);
            return false;
        }

        public void SetupScripts(params ScriptBase[] scripts)
        {
            _driver = gameObject.AddComponent<AiController>();
            if (_driver is AiController aiController)
            {
                aiController.Scripts = new Queue<ScriptBase>(scripts);
            }
        }

        #endregion

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        private void Update()
        {
            if (hull == null) return;
            _driver?.UpdateControl(this);
        }

        #region IAbbility
        public ItemAbilities[] Abilities = System.Array.Empty<ItemAbilities>();
        private List<ItemAbilities> _runtimeAbilities;

        public IReadOnlyList<ItemAbilities> RuntimeAbilities
        {
            get
            {
                _runtimeAbilities ??= new List<ItemAbilities>(Abilities ?? System.Array.Empty<ItemAbilities>());
                return _runtimeAbilities;
            }
        }

        public void AddAbility(ItemAbilities ability)
        {
            if (ability == null) return;
            ((List<ItemAbilities>)RuntimeAbilities).Add(ability);
        }

        public bool RemoveAbility(ItemAbilities ability) => ((List<ItemAbilities>)RuntimeAbilities).Remove(ability);

        public void Activate(Vector3 targetPos, params TemplateActionBase[] actions)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
