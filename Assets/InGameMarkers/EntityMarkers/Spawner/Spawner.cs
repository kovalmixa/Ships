using System.Collections.Generic;
using Assets.Entity;
using Assets.Handlers.SceneHandlers;
using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.InGameMarkers.EntityMarkers.Spawner
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
            EntityBody entityBody = other.GetComponent<EntityBody>();
            if (entityBody == null) return;
            if (!entityBody.EntityController.IsPlayer) return;
            if (!_entityObj || !_entityObj.activeSelf) Spawn();
        }

        //private void CheckTheDistanceToDeSpawn()
        //{
        //    if (_player == null || _entityObj == null) return;
        //    Vector2 playerPosition = _player.position;
        //    Vector2 entityPosition = _entityObj.transform.position;
        //    Collider2D playerCollider = _player.GetComponent<Collider2D>();
        //    float playerRadius = playerCollider != null ? playerCollider.bounds.extents.magnitude : 0f;
        //    CircleCollider2D circle = GetComponent<CircleCollider2D>();
        //    float spawnRadius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        //    float dist = Vector2.Distance(playerPosition, entityPosition);
        //    if (dist >= spawnRadius + playerRadius)
        //    {
        //        Destroy(_entityObj);
        //        _entityObj = null;
        //    }
        //}
        void Spawn()
        {
            _entityObj = _entityPool.GetComponent<ObjectPoolHandler>().Get();
            _entityObj.transform.position = transform.position;
            EntityController entityController = _entityObj.GetComponent<EntityController>();
            if (entityController != null)
            {
                entityController.Nation = Nation;
                SetupEntity(entityController);
            }
        }
        private void SetupEntity(EntityController entityController)
        {
            StartCoroutine(entityController.SetHull());
        }
    }
}
