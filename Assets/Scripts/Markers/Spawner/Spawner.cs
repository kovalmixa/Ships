using System.Collections.Generic;
using Assets.Entity;
using Assets.Entity.Equipment;
using Assets.Scripts.Actions;
using GameplayActions;
using Scripts;
using UnityEngine;
using Assets.Scripts.GameplayActions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EntityMarkers.Spawner
{
    [ExecuteInEditMode]
    public class Spawner : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] public EntityData data;
        [SerializeField] private uint _spawnQuantity = 1;
        [SerializeField] private ScriptBase[] _scripts;

        private readonly InteractionContext _context = new InteractionContext();
        private readonly SpawnData _spawnData = new SpawnData();

        #region Setup

        private void Awake()
        {
            if (!Application.isPlaying) return;

            ClearPreview();
            _context.SetSource(gameObject);
            _spawnData.entityData = data;
            _spawnData.scripts = _scripts;
        }

        private void Start()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < _spawnQuantity; i++)
            {
                ActionProvider.Spawn.Execute(_context, _spawnData, transform.position);
            }
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (!Application.isPlaying)
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (!Application.isPlaying) ClearPreview();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode) ClearPreview();
        }
#endif

        #endregion

        #region Editor Methods (Safe for Build)

        // Оставляем сигнатуру публичных методов открытой для компилятора,
        // но вырезаем логику внутри для билда игрока.
        public void ClearPreview()
        {
#if UNITY_EDITOR
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child != null) DestroyImmediate(child.gameObject);
            }

            var defaultSprite = GetComponent<SpriteRenderer>();
            if (defaultSprite != null) defaultSprite.enabled = true;
#endif
        }

        public void BuildPreviewInEditor(GameObject hullPrefab, List<GameObject> equipmentPrefabs)
        {
#if UNITY_EDITOR
            ClearPreview();

            if (hullPrefab == null) return;

            var previewInstance = Instantiate(hullPrefab, transform);
            previewInstance.name = hullPrefab.name + " (Preview)";
            previewInstance.transform.localPosition = Vector3.zero;
            previewInstance.transform.localRotation = Quaternion.identity;
            previewInstance.hideFlags = HideFlags.DontSave;

            var hull = previewInstance.GetComponent<Assets.Entity.Hull.HullBase>();
            if (hull == null || equipmentPrefabs == null) return;

            var defaultSprite = GetComponent<SpriteRenderer>();
            if (defaultSprite != null) defaultSprite.enabled = false;

            for (int i = 0; i < equipmentPrefabs.Count; i++)
            {
                var eqPrefab = equipmentPrefabs[i];
                if (eqPrefab == null) continue;

                var eqInstance = Instantiate(eqPrefab);
                eqInstance.hideFlags = HideFlags.DontSave;

                var equipment = eqInstance.GetComponentInChildren<Equipment>();
                if (equipment != null && i < hull.equipmentAnchors.Count)
                    hull.equipmentAnchors[i].Place(equipment);
                else
                    DestroyImmediate(eqInstance);
            }
#endif
        }

        #endregion
    }
}