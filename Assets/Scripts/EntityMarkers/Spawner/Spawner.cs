using System.Collections.Generic;
using Assets.DataContainers;
using Assets.Entity;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Scripts;
using UnityEngine;

namespace Assets.Scripts.EntityMarkers.Spawner
{
    public class Spawner : MonoBehaviour
    {
        public string Nation;
        public uint Level;
        public uint Quantity;
        private Transform _entityPool;
        private GameObject _entityObj;
        [SerializeField] public List<ScriptBase> ScriptList;
        private void Awake()
        {
            _entityPool = SceneNodesHandler.GetNode("ObjectPools").transform.Find("EntityPool");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Quantity == 0) return;
            var entityController = other.GetComponent<EntityController>();
            if (entityController == null) return;
            if (!entityController.IsPlayer) return;
            if (!_entityObj || !_entityObj.activeSelf) Spawn();
        }


        void Spawn()
        {
            _entityObj = _entityPool.GetComponent<ObjectPoolHandler>().Get();
            _entityObj.transform.position = transform.position;
            EntityController entityController = _entityObj.GetComponent<EntityController>();
            if (entityController != null)
            {
                //entityController.Nation = Nation;
                SetupEntity(entityController);
            }
        }
        private void SetupEntity(EntityController entityController)
        {
            
            entityController.Setup(GenerateHull());
            //entityController.SetEquipment();
        }

        private EntityDataContainer GenerateHull()
        {
            EntityDataContainer data = new()
            {
                HullId = "NONE/boat"
            };
            return data;
        }
    }
}
