#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using EntityMarkers.Spawner;
using System.Collections.Generic;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Spawner spawner = (Spawner)target;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) UpdatePreview(spawner);
        if (GUILayout.Button("Rebuild Preview")) UpdatePreview(spawner);
    }

    private void UpdatePreview(Spawner spawner)
    {
        if (spawner.preset == null)
        {
            spawner.ClearPreview();
            return;
        }

        // Поиск префабов в папках проекта по ID/Имени через AssetDatabase
        //GameObject hullPrefab = LoadPrefabById(spawner.preset.hullId);
        List<GameObject> eqPrefabs = new();

        //foreach (var eqData in spawner.preset.equipment)
        //    eqPrefabs.Add(LoadPrefabById(eqData.equipmentId));

        //spawner.BuildPreviewInEditor(hullPrefab, eqPrefabs);
    }

    private GameObject LoadPrefabById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        // Поиск по имени файла или пути в проекте
        string[] guids = AssetDatabase.FindAssets($"{id} t:Prefab");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        return null;
    }
}
#endif