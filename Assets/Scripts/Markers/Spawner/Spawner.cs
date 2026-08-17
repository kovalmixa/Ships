using Assets.Entity;
using Assets.Entity.Equipment;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers;
using Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityMarkers.Spawner
{
    [ExecuteInEditMode] // Позволяет скрипту выполнять логику в редакторе
    public class Spawner : MonoBehaviour
    {
        [Header("Configuration")]
        public NpcPreset preset; // Выбор готового комплекта
        public List<ScriptBase> ScriptList;

        [HideInInspector]
        [SerializeField] private GameObject _previewInstance;

        private GameObject _runtimeEntityObj;

        private void Awake()
        {
            ClearPreview();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!Application.isPlaying) return;

            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController == null || !GameObjectHandler.IsPlayer(entityController)) return;
            if (_runtimeEntityObj == null || !_runtimeEntityObj.activeSelf) Spawn();
        }

        public void Spawn()
        {
            _runtimeEntityObj.transform.position = transform.position;

            var entityController = _runtimeEntityObj.GetComponent<EntityController>();
            if (entityController != null && preset != null)
            {
                EntityData data = new()
                {
                    hullId = preset.hullId,
                    equipmentIds = ConvertEquipmentToDict(preset.equipment)
                };

                entityController.Setup(data);
                entityController.SetupAi(ScriptList.ToArray());
            }
        }

        private List<KeyValuePair<string, int>> ConvertEquipmentToDict(object equipment)
        {
            throw new NotImplementedException();
        }

        public void ClearPreview()
        {
            if (_previewInstance != null)
            {
                DestroyImmediate(_previewInstance);
                _previewInstance = null;
            }
        }

        private Dictionary<string, int> ConvertEquipmentToDict(List<EquipmentSlotData> list)
        {
            var dict = new Dictionary<string, int>();
            //foreach (var item in list) dict[item.equipmentId] = item.slotIndex;
            return dict;
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
            if (hull == null || equipmentPrefabs == null) return;

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