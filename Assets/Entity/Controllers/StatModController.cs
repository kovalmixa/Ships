using System.Collections.Generic;
using System.Linq;
using Assets.Entity.Modifiers;

namespace Assets.Entity.Controllers
{
    public static class StatCalculator
    {
        public static float Calculate(float baseValue, Modifiers.Modifiers.CompactMod mod)
        {
            if (mod == null || mod.IsEmpty) return baseValue;
            float result = mod.SetValue ?? baseValue;
            result += mod.Addition;
            result += result * (mod.Percentage / 100f);
            return result;
        }
    }

    public class StatModController
    {
        private bool _isDirty = true;
        public bool IsDirty => _isDirty;

        private Dictionary<(StatType Type, StatLayer Layer), float> _baseStats = new();

        private readonly Dictionary<(StatType Type, StatLayer Layer), float> _cachedCombinedStats = new();

        private readonly Modifiers.Modifiers _localModifiers = new();
        public Modifiers.Modifiers LocalModifiers => _localModifiers;

        private readonly List<Modifiers.Modifiers> _externalModifiers = new();
        private readonly StatModController _totalController;
        public StatModController(StatModController totalModStatController) => _totalController = totalModStatController;

        public void SetupBaseStats(IEnumerable<StatUnit> baseStats, IEnumerable<ModUnit> localMods = null)
        {
            _baseStats = baseStats.GroupBy(unit => (unit.Type, unit.StatLayer))
                .ToDictionary(g => g.Key, g => g.Sum(unit => unit.Value));

            _localModifiers.Clear();
            if (localMods != null) _localModifiers.Add(localMods);

            _isDirty = true;
        }

        public void RegisterExternalModifiers(Modifiers.Modifiers mods)
        {
            if (mods == null || _externalModifiers.Contains(mods)) return;
            _externalModifiers.Add(mods);
            _isDirty = true;
        }

        public void UnregisterExternalModifiers(Modifiers.Modifiers mods)
        {
            if (mods == null) return;
            _externalModifiers.Remove(mods);
            _isDirty = true;
        }

        public float GetStat(StatType type, StatLayer layer)
        {
            var key = (type, layer);
            if (_isDirty) RebuildCachedStats();

            if (_cachedCombinedStats.TryGetValue(key, out float value)) return value;
            return _baseStats.TryGetValue(key, out float baseValue) ? baseValue : 0f;
        }

        private void RebuildCachedStats()
        {
            _cachedCombinedStats.Clear();

            var totalActiveModifiers = new Modifiers.Modifiers();
            totalActiveModifiers.Add(_localModifiers);

            foreach (var extMod in _externalModifiers) totalActiveModifiers.Add(extMod);

            foreach (var kvp in _baseStats)
            {
                var key = kvp.Key;
                float baseValue = kvp.Value;
                var activeMod = totalActiveModifiers.GetMod(key.Type, key.Layer);
                float finalValue = StatCalculator.Calculate(baseValue, activeMod);
                _cachedCombinedStats[key] = finalValue;
            }
            _isDirty = false;
        }
    }
}