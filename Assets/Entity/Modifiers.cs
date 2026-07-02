using System.Collections.Generic;
using System.Linq;

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
        public bool IsGlobal { get; }
        public StatCalcType CalcType { get; }
        public float Value { get; }

        public StatMod(StatType type, bool isGlobal, StatCalcType calcType, float value)
        {
            Type = type;
            IsGlobal = isGlobal;
            CalcType = calcType;
            Value = value;
        }

        public StatMod WithValue(float newValue) => new(Type, IsGlobal, CalcType, newValue);
    }

    public class Modifiers
    {
        private readonly Dictionary<(StatType Type, bool IsGlobal), List<StatMod>> _modsMap = new();

        public IEnumerable<StatMod> StatsMods => _modsMap.Values.SelectMany(x => x);

        public void Add(Modifiers otherModifiers)
        {
            if (otherModifiers == null) return;
            foreach (var incomingMod in otherModifiers.StatsMods) AddSingle(incomingMod);
        }

        public void AddSingle(StatMod mod)
        {
            var key = (mod.Type, mod.IsGlobal);

            if (!_modsMap.TryGetValue(key, out var list))
            {
                list = new List<StatMod>();
                _modsMap[key] = list;
            }
            int existingIndex = list.FindIndex(m => m.CalcType == mod.CalcType);
            if (existingIndex != -1) list[existingIndex] = list[existingIndex].WithValue(list[existingIndex].Value + mod.Value);
            else list.Add(mod);
        }

        public float ApplyModByType(StatType type, float basicValue, bool? getGlobal = null)
        {
            float currentSum = basicValue;
            float totalAddition = 0f;
            float totalPercent = 0f;
            bool anyModApplied = false;

            if (getGlobal == null || getGlobal == true)
                ProcessKey((type, true), ref currentSum, ref totalAddition, ref totalPercent, ref anyModApplied);
            if (getGlobal == null || getGlobal == false)
                ProcessKey((type, false), ref currentSum, ref totalAddition, ref totalPercent, ref anyModApplied);
            if (!anyModApplied) return basicValue;

            currentSum += totalAddition;
            currentSum += currentSum * (totalPercent / 100f);

            return currentSum;
        }

        private void ProcessKey((StatType Type, bool IsGlobal) key, ref float currentSum, ref float totalAddition, ref float totalPercent, ref bool anyModApplied)
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