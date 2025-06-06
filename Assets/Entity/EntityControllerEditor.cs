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
            controller.HullId = EditorGUILayout.TextField("Hull ID", controller.HullId);
            if (!controller.IsPlayer)
            {
                controller.Type = EditorGUILayout.TextField("Type", controller.Type);
                SerializedProperty scripts = serializedObject.FindProperty("ScriptList");
                EditorGUILayout.PropertyField(scripts, new GUIContent("Script List"), true);
                SerializedProperty weaponIds = serializedObject.FindProperty("WeaponIds");
                EditorGUILayout.PropertyField(weaponIds, new GUIContent("Weapon IDs"), true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}