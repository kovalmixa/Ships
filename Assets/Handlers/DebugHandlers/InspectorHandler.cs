#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Assets.Handlers.DebugHandlers
{
    public static class InspectorHandler
    {
        public static GameObject LoadPrefabById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                foreach (AddressableAssetGroup group in settings.groups)
                {
                    if (group == null) continue;

                    foreach (AddressableAssetEntry entry in group.entries)
                        if (entry.address == id) return AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
                }
            }

            string fileName = id.Contains("/") ? id.Substring(id.LastIndexOf("/") + 1) : id;
            string[] guids = AssetDatabase.FindAssets($"{fileName} t:Prefab");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }

            Debug.LogWarning($"[InspectorHandler] Префаб с ID '{id}' не найден ни в Addressables, ни по имени файла.");
            return null;
        }
    }
}
#endif