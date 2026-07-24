namespace Assets.Handlers.Editor
{
    using Assets.Entity.Modifiers;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(StatType))]
    public class StatTypeDrawer : PropertyDrawer
    {
        private static readonly string[] Categories = new string[]
        { "Resources", "Movement", "Attack", "Defense", "Elements", "Resistances", "Crits", "Projectiles", "Economy" };

        private static readonly Dictionary<int, List<StatType>> CategoryMap = new Dictionary<int, List<StatType>>();
        private enum StatCategory { Resources, Movement, Attack, Defense, Elements, Resists, Crits, Projectiles, Economy }

        static StatTypeDrawer()
        {
            foreach (int categoryIndex in Enum.GetValues(typeof(StatCategory)))
                CategoryMap[categoryIndex] = new List<StatType>();

            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                int val = (int)stat;
                int cat = val switch
                {
                    >= 0 and < 50 => 0, // Resources
                    >= 50 and < 100 => 1, // Movement
                    >= 100 and < 150 => 2, // Attack
                    >= 150 and < 200 => 3, // Defense
                    >= 200 and < 250 => 4, // Elements
                    >= 250 and < 300 => 5, // Resistances
                    >= 300 and < 350 => 6, // Critical Hits
                    >= 350 and < 400 => 7, // Projectiles
                    _ => 8 // Economy / Rewards
                };

                CategoryMap[cat].Add(stat);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            StatType currentValue = (StatType)property.intValue;
            int currentCat = GetCategoryForStat(currentValue);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float spacing = 4f;
            float halfWidth = (position.width - spacing) / 2f;

            Rect categoryRect = new Rect(position.x, position.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect statRect = new Rect(position.x + halfWidth + spacing, position.y, halfWidth, EditorGUIUtility.singleLineHeight);

            int newCat = EditorGUI.Popup(categoryRect, currentCat, Categories);

            if (newCat != currentCat)
            {
                currentCat = newCat;
                currentValue = CategoryMap[currentCat][0];
                property.intValue = (int)currentValue;
            }

            List<StatType> categoryStats = CategoryMap[currentCat];
            string[] names = categoryStats.ConvertAll(s => s.ToString()).ToArray();

            int selectedIndex = categoryStats.IndexOf(currentValue);
            if (selectedIndex == -1) selectedIndex = 0;

            int newIndex = EditorGUI.Popup(statRect, selectedIndex, names);

            property.intValue = (int)categoryStats[newIndex];

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        private int GetCategoryForStat(StatType stat)
        {
            int val = (int)stat;
            return val switch
            {
                >= 0 and < 50 => 0,
                >= 50 and < 100 => 1,
                >= 100 and < 150 => 2,
                >= 150 and < 200 => 3,
                >= 200 and < 250 => 4,
                >= 250 and < 300 => 5,
                >= 300 and < 350 => 6,
                >= 350 and < 400 => 7,
                _ => 8
            };
        }

    }
}
