using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Entity.Modifiers
{
    public enum StatLayer { Hull, Equipment, Projectile, Global }
   
    public enum StatCalcType { Set = 0, Addition = 1, Percentage = 2 }

    public enum StatType
    {
        Hp, MaxHp, Energy, EnergyPercent, MaxMoveSpeed, RotationSpeed,
        Mass, Acceleration, Armor, FireResistance, FireRate, Penetration,
        Damage, PrSpeed, ElementalChance, CritChance, PrMoveType, PrIsHoming, PrLifeTime, MaxRange
    }

    [System.Serializable]
    public class StatUnit
    {
        public StatType Type { get; }
        public StatLayer StatLayer { get; }
        public float Value { get; }

        public StatUnit(StatType type, StatLayer layer, float value)
        {
            Type = type;
            StatLayer = layer;
            Value = value;
        }

        public StatUnit WithValue(float newValue) => new(Type, StatLayer, newValue);
    }
    
    [System.Serializable]
    public class ModUnit : StatUnit
    {
        public StatCalcType CalcType { get; }

        public ModUnit(StatType type, StatLayer layer, StatCalcType calcType, float value)
            : base(type, layer, value)
        {
            CalcType = calcType;
        }

        public new ModUnit WithValue(float newValue) => new(Type, StatLayer, CalcType, newValue);
    }

    public class Modifiers
    {
        private class CompactMod
        {
            public float? SetValue { get; set; }
            public float Addition { get; set; }
            public float Percentage { get; set; }

            public bool IsEmpty => SetValue == null && Addition == 0f && Percentage == 0f;
        }

        private readonly Dictionary<(StatType Type, StatLayer Layer), CompactMod> _modsMap = new();

        public IEnumerable<ModUnit> StatsMods => _modsMap.SelectMany(kvp =>
        {
            var (type, layer) = kvp.Key;
            var mod = kvp.Value;
            var list = new List<ModUnit>();

            if (mod.SetValue.HasValue) list.Add(new ModUnit(type, layer, StatCalcType.Set, mod.SetValue.Value));
            if (mod.Addition != 0f) list.Add(new ModUnit(type, layer, StatCalcType.Addition, mod.Addition));
            if (mod.Percentage != 0f) list.Add(new ModUnit(type, layer, StatCalcType.Percentage, mod.Percentage));

            return list;
        });

        public Modifiers() { }
        public Modifiers(IEnumerable<ModUnit> modUnits) => Add(modUnits);

        public void Add(IEnumerable<ModUnit> modUnits)
        {
            if (modUnits == null) return;
            foreach (var mod in modUnits) AddSingle(mod);
        }

        public void Add(Modifiers otherModifiers)
        {
            if (otherModifiers == null) return;
            foreach (var incomingMod in otherModifiers.StatsMods) AddSingle(incomingMod);
        }

        public void AddSingle(ModUnit mod)
        {
            var key = (mod.Type, mod.StatLayer);

            if (!_modsMap.TryGetValue(key, out var compactMod))
            {
                compactMod = new CompactMod();
                _modsMap[key] = compactMod;
            }

            switch (mod.CalcType)
            {
                case StatCalcType.Set:
                    compactMod.SetValue = mod.Value;
                    break;
                case StatCalcType.Addition:
                    compactMod.Addition += mod.Value;
                    break;
                case StatCalcType.Percentage:
                    compactMod.Percentage += mod.Value;
                    break;
            }
        }

        public float ApplyModByType(StatType type, StatLayer layer, float basicValue)
        {
            if (!_modsMap.TryGetValue((type, layer), out var mod) || mod.IsEmpty)
                return basicValue;
            float result = mod.SetValue ?? basicValue;

            result += mod.Addition;
            result += result * (mod.Percentage / 100f);

            return result;
        }

        public void Clear() => _modsMap.Clear();
    }
}