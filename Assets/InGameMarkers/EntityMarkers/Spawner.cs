using System;
using System.Collections.Generic;
using Assets.Entity;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Assets.InGameMarkers.EntityMarkers
{
    public class Spawner : MonoBehaviour
    {
        public string Nation;
        public uint Level;
        public uint Quantity;
        private Transform _player;
        private Transform _entityPool;
        private GameObject _entityObj;
        public GameObject EntityPrefab;
        [SerializeField] public List<ScriptBase> ScriptList;
        private void Start()
        {
            try
            {
                GameObject player = FindRootObjectByName("Player");
                if (player == null) throw (new Exception("Player is not found among root of scene"));
                _player = player.transform;
                GameObject entityPool = FindRootObjectByName("Entities");
                if (entityPool == null) throw (new Exception("Entity pull is not found in root of scene"));
                _entityPool = entityPool.transform;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception.Message);
            }

        }
        GameObject FindRootObjectByName(string name)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                if (obj.name == name)
                    return obj;
            }

            return null;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Quantity == 0) return;
            if (!other.GetComponent<Entity.Entity>().EntityController.IsPlayer) return;
            if (!_entityObj) Spawn();
        }
        void Update() => CheckTheDistanceToDeSpawn();
        private void CheckTheDistanceToDeSpawn()
        {
            if (_player == null || _entityObj == null) return;
            Vector2 playerPosition = _player.position;
            Vector2 entityPosition = _entityObj.transform.position;
            Collider2D playerCollider = _player.GetComponent<Collider2D>();
            float playerRadius = playerCollider != null ? playerCollider.bounds.extents.magnitude : 0f;
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            float spawnRadius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            float dist = Vector2.Distance(playerPosition, entityPosition);
            if (dist >= spawnRadius + playerRadius)
            {
                Destroy(_entityObj);
                _entityObj = null;
            }
        }
        void Spawn()
        {
            _entityObj = Instantiate(EntityPrefab, transform.position, Quaternion.identity, _entityPool);
            EntityController entityController = _entityObj.GetComponent<EntityController>();
            if (entityController != null)
            {
                entityController.Nation = Nation;
                SetupEntity(entityController);
            }
        }
        private void SetupEntity(EntityController entityController)
        {
            entityController.SetHull();
        }
    }
}
