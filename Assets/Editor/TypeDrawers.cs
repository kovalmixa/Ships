namespace Assets.Handlers.Editor
{
    using Assets.Entity.Modifiers;
    using Assets.Scripts.Actions.VFX;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public enum StatCategory { Resources, Movement, Attack, Defense, Elements, Resists, Crits, Projectiles, Economy }
    public enum VfxCategory { Bullet }


    public class TypeDrawerCache<SubCat, MasterCat>
        where SubCat : struct, Enum
        where MasterCat : struct, Enum
    {
        private static TypeDrawerCache<SubCat, MasterCat> _instance;

        public static TypeDrawerCache<SubCat, MasterCat> GetInstance(int catDifference) => 
            _instance ??= new TypeDrawerCache<SubCat, MasterCat>(catDifference);

        public string[] MasterCategoryNames { get; }
        public Dictionary<int, List<SubCat>> CategoryMap { get; } = new();
        public Dictionary<int, string[]> SubCategoryNamesMap { get; } = new();

        private TypeDrawerCache(int catDifference)
        {
            MasterCategoryNames = Enum.GetNames(typeof(MasterCat));

            int masterCount = MasterCategoryNames.Length;
            for (int i = 0; i < masterCount; i++) CategoryMap[i] = new List<SubCat>();
            foreach (SubCat stat in Enum.GetValues(typeof(SubCat)))
                CategoryMap[GetCategoryForStat(stat, catDifference, masterCount)].Add(stat);

            foreach (var kvp in CategoryMap)
                SubCategoryNamesMap[kvp.Key] = kvp.Value.ConvertAll(s => s.ToString()).ToArray();
        }

        public int GetCategoryForStat(SubCat stat, int catDifference, int maxCategories)
        {
            int statValue = Convert.ToInt32(stat);
            int category = statValue / catDifference;
            return Mathf.Clamp(category, 0, Math.Max(0, maxCategories - 1));
        }
    }

    public class TypeDrawer<SubCat, MasterCat> : PropertyDrawer
        where SubCat : struct, Enum
        where MasterCat : struct, Enum
    {
        private readonly int _catDifference;

        protected TypeDrawer(int catDifference) => _catDifference = catDifference;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var cache = TypeDrawerCache<SubCat, MasterCat>.GetInstance(_catDifference);

            SubCat currentValue = (SubCat)(object)property.intValue;
            int currentCat = cache.GetCategoryForStat(currentValue, _catDifference, cache.MasterCategoryNames.Length);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float spacing = 4f;
            float halfWidth = (position.width - spacing) / 2f;

            Rect categoryRect = new Rect(position.x, position.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect statRect = new Rect(position.x + halfWidth + spacing, position.y, halfWidth, EditorGUIUtility.singleLineHeight);

            int newCat = EditorGUI.Popup(categoryRect, currentCat, cache.MasterCategoryNames);
            if (newCat != currentCat)
            {
                currentCat = newCat;
                var list = cache.CategoryMap[currentCat];
                if (list.Count > 0)
                {
                    currentValue = list[0];
                    property.intValue = Convert.ToInt32(currentValue);
                }
            }

            List<SubCat> categoryStats = cache.CategoryMap[currentCat];
            string[] names = cache.SubCategoryNamesMap[currentCat];

            if (categoryStats.Count > 0)
            {
                int selectedIndex = categoryStats.IndexOf(currentValue);
                if (selectedIndex == -1) selectedIndex = 0;

                int newIndex = EditorGUI.Popup(statRect, selectedIndex, names);
                property.intValue = Convert.ToInt32(categoryStats[newIndex]);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;
    }

    [CustomPropertyDrawer(typeof(StatType))]
    public class StatTypeDrawer : TypeDrawer<StatType, StatCategory>
    {
        public StatTypeDrawer() : base(50) { }
    }

    [CustomPropertyDrawer(typeof(VfxType))]
    public class VfxTypeDrawer : TypeDrawer<VfxType, VfxCategory>
    {
        public VfxTypeDrawer() : base(10) { }
    }
}