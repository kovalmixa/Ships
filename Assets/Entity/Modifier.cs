using System.Collections.Generic;
using System.Linq;

namespace Assets.Entity.Modifiers
{
    public enum ModifierStatCalcType
    {
        Set = 0,
        Addition = 1,
        Percentage = 2
    }

    public class ModifierStat
    {
        public string Name { get; set; }
        public ModifierStatCalcType Type { get; set; }
        public float Value { get; set; }

        public ModifierStat Clone() => new() { Name = Name, Type = Type, Value = Value };
    }

    public class Modifier
    {
        public List<ModifierStat> Stats { get; private set; } = new();

        public void Add(Modifier modifier)
        {
            foreach (var stat in modifier.Stats)
            {
                var targetStat = Stats.FirstOrDefault(s => s.Name == stat.Name && s.Type == stat.Type);
                if (targetStat != null) targetStat.Value += stat.Value;
                else Stats.Add(stat.Clone());
            }
        }

        public float Calculate(string name, float basicValue)
        {
            var statsByName = Stats.Where(s => s.Name == name).ToList();
            if (!statsByName.Any()) return basicValue;

            var setStat = statsByName.LastOrDefault(s => s.Type == ModifierStatCalcType.Set);
            float currentSum = setStat != null ? setStat.Value : basicValue;
            float totalAddition = statsByName.Where(s => s.Type == ModifierStatCalcType.Addition).Sum(s => s.Value);
            currentSum += totalAddition;
            float totalPercent = statsByName.Where(s => s.Type == ModifierStatCalcType.Percentage).Sum(s => s.Value);
            currentSum += currentSum * totalPercent / 100f;
            return currentSum;
        }
    }
}