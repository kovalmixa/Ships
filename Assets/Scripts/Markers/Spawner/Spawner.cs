using Assets.Entity.Equipment;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Assets.Scripts.GameplayActions;
using Assets.Scripts.Markers.Spawner;
using GameplayActions;
using Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace EntityMarkers.Spawner
{
    [ExecuteInEditMode]
    public class Spawner : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] public NpcData data;
        [SerializeField] private ScriptBase[] _scripts;

        [HideInInspector]
        [SerializeField] private GameObject _previewInstance;

        private SpriteRenderer _defaultSprite;

        private uint _spawnQuantity;
        private bool _isSpawned = false;
        private InteractionContext _context = new InteractionContext();
        private SpawnData _spawnData = new SpawnData();

        private void Awake()
        {
            _defaultSprite = GetComponent<SpriteRenderer>();
            ClearPreview();

            _context.SetSource(gameObject);
            _spawnData.npcData = data;
            _spawnData.scripts = _scripts;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!Application.isPlaying || _isSpawned) return;

            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController == null || !GameObjectHandler.IsPlayer(entityController)) return;
            for(int i = 0; i < _spawnQuantity; i++) Spawn(); 
        }

        private void Spawn()
        {
            ActionProvider.Spawn.Execute(_context, _spawnData, transform.position);
            _isSpawned = true;
        }

        public void ClearPreview()
        {
            if (_previewInstance != null)
            {
                DestroyImmediate(_previewInstance);
                _previewInstance = null;
            }
        }

#if UNITY_EDITOR
        public void BuildPreviewInEditor(GameObject hullPrefab, List<GameObject> equipmentPrefabs)
        {
            ClearPreview();

            if (hullPrefab == null) return;

            _previewInstance = UnityEditor.PrefabUtility.InstantiatePrefab(hullPrefab, transform) as GameObject;
            _previewInstance.transform.localPosition = Vector3.zero;
            _previewInstance.transform.localRotation = Quaternion.identity;
            _previewInstance.hideFlags = HideFlags.DontSaveInBuild;

            var hull = _previewInstance.GetComponent<Assets.Entity.Hull.HullBase>();
            if (hull == null || equipmentPrefabs == null)
            {
                _defaultSprite.enabled = true;
                return;
            }
            _defaultSprite.enabled = false;

            for (int i = 0; i < equipmentPrefabs.Count; i++)
            {
                var eqPrefab = equipmentPrefabs[i];
                if (eqPrefab == null) continue;

                var eqInstance = UnityEditor.PrefabUtility.InstantiatePrefab(eqPrefab) as GameObject;
                var equipment = eqInstance.GetComponentInChildren<Equipment>();

                if (equipment != null && i < hull.equipmentAnchors.Count)
                    hull.equipmentAnchors[i].Place(equipment);
                else DestroyImmediate(eqInstance);
            }
        }
#endif
    }
}