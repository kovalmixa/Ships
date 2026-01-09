using System.Collections.Generic;
using Assets.DataContainers;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.GenericController;
using Scripts;
using UnityEngine;

namespace EntityMarkers.Spawner
{
    public class Spawner : MonoBehaviour
    {
        public string Nation;
        public uint Level;
        public uint Quantity = 1;
        private ObjectPoolHandler _entityPool;
        private GameObject _entityObj;
        [SerializeField] public List<ScriptBase> ScriptList;

        private void Awake()
        {
            _entityPool = SceneNodesHandler.GetPoolHandler("EntityPool");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController == null) return;
            if (!entityController.isPlayer) return;
            //Debug.Log(_entityObj == null);
            if (_entityObj == null || !_entityObj.activeSelf) Spawn();
        }

        void Spawn()
        {
            _entityObj = _entityPool.Get();
            _entityObj.transform.position = transform.position;
            EntityController entityController = _entityObj.GetComponent<EntityController>();
            if (entityController != null)
            {
                //entityController.Nation = Nation;
                SetupEntity(entityController);
                entityController.SetupScripts(ScriptList.ToArray());
            }
        }

        private void SetupEntity(EntityController entityController)
        {
            
            entityController.Setup(GenerateHull());
            SetIdentityRotation(entityController);
            //entityController.SetEquipment();
        }

        private void SetIdentityRotation(EntityController entityController)
        {
            entityController.transform.rotation = Quaternion.identity;
            entityController.transform.position = transform.position;

            if (entityController.hull != null)
            {
                entityController.hull.transform.rotation = Quaternion.identity;
                entityController.hull.transform.position = entityController.transform.position;
            }
        }

        private EntityDataContainer GenerateHull()
        {
            EntityDataContainer data = new()
            {
                EquipmentIds = new (),
                HullId = "NONE/boat"
            };
            return data;
        }
    }
}
