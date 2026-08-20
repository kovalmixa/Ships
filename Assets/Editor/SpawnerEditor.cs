#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using EntityMarkers.Spawner;
using System.Collections.Generic;
using Assets.Handlers.DebugHandlers;

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
        if (spawner.data == null)
        {
            spawner.ClearPreview();
            return;
        }
        var enityData = spawner.data.entityData;
        GameObject hullPrefab = InspectorHandler.LoadPrefabById(enityData.hullId);
        List<GameObject> eqPrefabs = new();

        foreach (var eqData in enityData.equipmentSlots)
            eqPrefabs.Add(InspectorHandler.LoadPrefabById(eqData.equipmentId));

        spawner.BuildPreviewInEditor(hullPrefab, eqPrefabs);
    }
}
#endif