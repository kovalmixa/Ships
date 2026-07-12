using Assets.Entity.Modifiers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class StatModController
    {
        private bool _isDirty { get; set; } = true;
        public bool IsDirty => _isDirty;

        private Modifiers.Modifiers _localModifiers;
        public Modifiers.Modifiers LocalModifiers => _localModifiers;

        private List<Modifiers.Modifiers> _externalModifiers = new();
        private Dictionary<(StatType Type, StatLayer Layer), float> _cachedCombinedStats = new();

        public void SetupStatsMods(Dictionary<(StatType Type, StatLayer Layer), float> baseStats, 
            Modifiers.Modifiers modifiers) => _localModifiers = modifiers;

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

        public float GetStat((StatType Type, StatLayer Layer) key)
        {
            if (_isDirty) RebuildCachedStats();
            return _cachedCombinedStats.TryGetValue(key, out float value) ? value : 1f;
        }

        private void RebuildCachedStats()
        {
            _cachedCombinedStats.Clear();
            var finalMods = new Modifiers.Modifiers();
            finalMods.Add(LocalModifiers);

            foreach (var extMod in _externalModifiers) finalMods.Add(extMod);
            var keys = new List<(StatType Type, StatLayer Layer)>(_cachedCombinedStats.Keys);
            foreach (var key in keys) _cachedCombinedStats[key] = finalMods.ApplyModByType(key.Type, key.Layer, _cachedCombinedStats[key]);

            _isDirty = false;
        }
    }
}
