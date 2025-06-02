using System;
using System.Collections.Generic;
using Assets.Entity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.GameObjects.InGameMarkers.Scripts
{
    public class SpawnScript : MonoBehaviour
    {
        public int SpawnDistance;
        public string Nation;
        public int Level;
        private Transform player;
        private Transform entityPool;
        private GameObject entityObj;
        public GameObject EntityPrefab { get; set; }
        [SerializeField] public List<GameObject> RouteScriptsList;
        [SerializeField] public List<GameObject> ScriptAreaList;
        [SerializeField] public List<GameObject> ScriptList;

        private void Start()
        {
            try
            {
                GameObject player = FindRootObjectByName("Player");
                if (player == null) throw (new Exception("Player is not found among root of scene"));
                GameObject entityPool = FindRootObjectByName("Entities");
                if (entityPool == null) throw (new Exception("Entity pull is not found in root of scene"));
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
        void Update()
        {
            if (player == null) return;
            float dist;
            if (!entityObj)
            {
                dist = Vector3.Distance(player.position, transform.position);
                if (dist <= SpawnDistance)
                {
                    Spawn();
                }
            }
            else
            {
                dist = Vector3.Distance(player.position, entityObj.transform.position);
                if (dist >= SpawnDistance)
                {
                    Destroy(entityObj);
                }
            }
        }
        void Spawn()
        {
            entityObj = Instantiate(EntityPrefab, transform.position, Quaternion.identity, entityPool);
            EntityController entityController = entityObj.GetComponent<EntityController>();
            if (entityController != null)
            {
                entityController.Nation = Nation;
                SetupEntity(entityController);
                ConnectEntityToScripts(entityController);
            }
        }
        private void SetupEntity(EntityController entityController)
        {
            entityController.SetHull();
        }
        private void ConnectEntityToScripts(EntityController entityController)
        {
            entityController.RouteScriptsList = RouteScriptsList;
            entityController.ScriptAreaList = ScriptAreaList;
            entityController.ScriptList = ScriptList;
        }
    }
}
