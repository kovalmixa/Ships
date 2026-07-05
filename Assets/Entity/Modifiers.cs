using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Entity.Modifiers
{
    public enum StatLayer
    {
        Hull,
        Equipment,
        Projectile,
        Global
    }

    public enum StatCalcType
    {
        Set = 0,
        Addition = 1,
        Percentage = 2
    }

    public enum StatType
    {
        Hp, HpPercent, Energy, EnergyPercent,
        MoveSpeed, RotationSpeed, Mass,
        Armor, FireResistance,
        FireRate, Penetration, Damage, ProjectileSpeed,
        FireChance, CritChance
    }

    public readonly struct StatMod
    {
        public StatType Type { get; }
        public StatLayer Layer { get; }
        public StatCalcType CalcType { get; }
        public float Value { get; }

        public StatMod(StatType type, StatLayer layer, StatCalcType calcType, float value)
        {
            Type = type;
            Layer = layer;
            CalcType = calcType;
            Value = value;
        }

        public StatMod WithValue(float newValue) => new(Type, Layer, CalcType, newValue);
    }

    public class Modifiers
    {
        private readonly Dictionary<(StatType Type, StatLayer Layer), List<StatMod>> _modsMap = new();

        public IEnumerable<StatMod> StatsMods => _modsMap.Values.SelectMany(x => x);

        public void Add(Modifiers otherModifiers)
        {
            if (otherModifiers == null) return;
            foreach (var incomingMod in otherModifiers.StatsMods) AddSingle(incomingMod);
        }

        public void AddSingle(StatMod mod)
        {
            var key = (mod.Type, mod.Layer);

            if (!_modsMap.TryGetValue(key, out var list))
            {
                list = new List<StatMod>();
                _modsMap[key] = list;
            }
            int existingIndex = list.FindIndex(m => m.CalcType == mod.CalcType);
            if (existingIndex != -1) list[existingIndex] = list[existingIndex].WithValue(list[existingIndex].Value + mod.Value);
            else list.Add(mod);
        }

        public float ApplyModByType(StatType type, StatLayer layer, float basicValue)
        {
            float currentSum = basicValue;
            float totalAddition = 0f;
            float totalPercent = 0f;
            bool anyModApplied = false;

            ProcessKey((type, layer), ref currentSum, ref totalAddition, ref totalPercent, ref anyModApplied);
            if (!anyModApplied) return basicValue;

            currentSum += totalAddition;
            currentSum += currentSum * (totalPercent / 100f);

            return currentSum;
        }

        private void ProcessKey((StatType Type, StatLayer Layer) key, ref float currentSum, ref float totalAddition, ref float totalPercent, ref bool anyModApplied)
        {
            if (!_modsMap.TryGetValue(key, out var mods)) return;
            anyModApplied = true;
            for (int i = 0; i < mods.Count; i++)
            {
                var mod = mods[i];
                switch (mod.CalcType)
                {
                    case StatCalcType.Set:
                        currentSum = mod.Value;
                        break;
                    case StatCalcType.Addition:
                        totalAddition += mod.Value;
                        break;
                    case StatCalcType.Percentage:
                        totalPercent += mod.Value;
                        break;
                }
            }
        }

        public void Clear() => _modsMap.Clear();
    }
}