using System.Collections.Generic;
using System.Linq;

namespace Assets.Entity.Modifiers
{
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

    public class StatMod
    {
        public StatType Type { get; set; }
        public StatCalcType CalcType { get; set; }
        public float Value { get; set; }

        public StatMod Clone() => new() { Type = Type, CalcType = CalcType, Value = Value };
    }

    public class Modifiers
    {
        public List<StatMod> StatsMods { get; private set; } = new();

        public void Add(Modifiers modifiers)
        {
            foreach (var stat in modifiers.StatsMods)
            {
                var targetStat = StatsMods.FirstOrDefault(s => s.Type == stat.Type && s.Type == stat.Type);
                if (targetStat != null) targetStat.Value += stat.Value;
                else StatsMods.Add(stat.Clone());
            }
        }

        public float ApplyModByType(StatType type, float basicValue)
        {
            var statsByName = StatsMods.Where(s => s.Type == type).ToList();
            if (!statsByName.Any()) return basicValue;

            var setStat = statsByName.LastOrDefault(s => s.CalcType == StatCalcType.Set);
            float currentSum = setStat != null ? setStat.Value : basicValue;
            float totalAddition = statsByName.Where(s => s.CalcType == StatCalcType.Addition).Sum(s => s.Value);
            currentSum += totalAddition;
            float totalPercent = statsByName.Where(s => s.CalcType == StatCalcType.Percentage).Sum(s => s.Value);
            currentSum += currentSum * totalPercent / 100f;
            return currentSum;
        }
    }
}