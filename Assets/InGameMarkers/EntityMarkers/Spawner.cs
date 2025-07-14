using System;
using System.Collections.Generic;
using Assets.Entity;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Assets.InGameMarkers.EntityMarkers
{
    public class Spawner : MonoBehaviour
    {
        public string Nation;
        public uint Level;
        public uint Quantity;
        private Transform _entityPool;
        private GameObject _entityObj;
        public GameObject EntityPrefab;
        [SerializeField] public List<ScriptBase> ScriptList;

        private void Start()
        {
            _entityPool = FindRootObjectByName("ObjectPools").transform.Find("EntityPool");
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
            Entity.EntityBody entityBody = other.GetComponent<Entity.EntityBody>();
            if (entityBody == null) return;
            if (!entityBody.EntityController.IsPlayer) return;
            if (!_entityObj) Spawn();
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
            StartCoroutine(entityController.SetHull());
        }
    }
}
