using UnityEditor;
using UnityEngine;

namespace Assets.Entity
{
    [CustomEditor(typeof(EntityController))]
    public class EntityControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EntityController controller = (EntityController)target;

            controller.AssetObjectPath = EditorGUILayout.TextField("Asset Object Path", controller.AssetObjectPath);
            controller.HullId = EditorGUILayout.TextField("Hull ID", controller.HullId);

            if (!controller.IsPlayer)
            {
                controller.Type = EditorGUILayout.TextField("Type", controller.Type);

                SerializedProperty routeScripts = serializedObject.FindProperty("RouteScriptsList");
                EditorGUILayout.PropertyField(routeScripts, new GUIContent("Route Scripts List"), true);

                SerializedProperty scriptAreas = serializedObject.FindProperty("ScriptAreaList");
                EditorGUILayout.PropertyField(scriptAreas, new GUIContent("Script Area List"), true);

                SerializedProperty scripts = serializedObject.FindProperty("ScriptList");
                EditorGUILayout.PropertyField(scripts, new GUIContent("Script List"), true);

                SerializedProperty weaponIds = serializedObject.FindProperty("WeaponIds");
                EditorGUILayout.PropertyField(weaponIds, new GUIContent("Weapon IDs"), true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}