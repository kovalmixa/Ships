using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Entity.Equipment
{
    public class AbilityPositionBaker : MonoBehaviour
    {
        [Header("Data to write")]
        [Tooltip("Config file to write coordinates to")]
        public EquipmentDataSO targetSO;

        [Tooltip("Ability index in the list (0 is the first ability)")]
        public int abilityIndex = 0;

        [Tooltip("That same visual node on the scene")]
        public Transform shootNode;

#if UNITY_EDITOR
        [ContextMenu("Move coordinates to SO")]
        public void BakePositionToSO()
        {
            if (targetSO == null || shootNode == null)
            {
                Debug.LogError("Assign Target SO and Shoot Node in the inspector!");
                return;
            }

            Undo.RecordObject(targetSO, "Bake Ability Position");
            Vector3 localPos = transform.InverseTransformPoint(shootNode.position);
            var abilities = targetSO.statOptions.abilities;
            if (abilityIndex >= 0 && abilityIndex < abilities.Count)
            {
                var ability = abilities[abilityIndex];
                ability.abilityPosition = new Vector2(localPos.x, localPos.y);
                abilities[abilityIndex] = ability;

                EditorUtility.SetDirty(targetSO);
                AssetDatabase.SaveAssets();

                Debug.Log($"<color=green>[Success]</color> Coordinates ({localPos.x:F2}, {localPos.y:F2}) written to {targetSO.name}!");
            }
            else
            {
                Debug.LogError($"Ability with index {abilityIndex} not found in SO!");
            }
        }

#endif
    }
}