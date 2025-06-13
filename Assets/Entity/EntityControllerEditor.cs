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
            SerializedProperty hullId = serializedObject.FindProperty("HullId");
            EditorGUILayout.PropertyField(hullId, new GUIContent("Hull ID"));
            SerializedProperty weaponIds = serializedObject.FindProperty("EquipmentIds");
            EditorGUILayout.PropertyField(weaponIds, new GUIContent("Weapon IDs"), true);
            if (!controller.IsPlayer)
            {
                SerializedProperty type = serializedObject.FindProperty("Type");
                EditorGUILayout.PropertyField(type, new GUIContent("Type"));
                SerializedProperty scripts = serializedObject.FindProperty("ScriptList");
                EditorGUILayout.PropertyField(scripts, new GUIContent("Script List"), true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}