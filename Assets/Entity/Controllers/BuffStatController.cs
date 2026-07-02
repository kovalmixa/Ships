using System.Collections.Generic;
using Assets.Entity.Modifiers;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class BuffStatController : MonoBehaviour
    {
        public Dictionary<(StatType Type, bool IsGlobal), float> BaseStats { get; set; } = new();
        public Modifiers.Modifiers LocalModifiers { get; set; } = new();
        public List<BuffStatus> BuffStatuses { get; set; } = new();
        private List<Modifiers.Modifiers> _externalModifiers = new();

        public bool IsDirty { get; set; } = true;
        private Dictionary<(StatType Type, bool IsGlobal), float> _cachedCombinedStats = new();

        public void RegisterExternalModifiers(Modifiers.Modifiers mods)
        {
            if (mods == null || _externalModifiers.Contains(mods)) return;
            _externalModifiers.Add(mods);
            IsDirty = true;
        }

        public void UnregisterExternalModifiers(Modifiers.Modifiers mods)
        {
            if (mods == null) return;
            _externalModifiers.Remove(mods);
            IsDirty = true;
        }

        public float GetStat((StatType Type, bool IsGlobal) key)
        {
            if (IsDirty) RebuildCachedStats();
            return _cachedCombinedStats.TryGetValue(key, out float value) ? value : 1f;
        }

        private void RebuildCachedStats()
        {
            _cachedCombinedStats.Clear();
            foreach (var stat in BaseStats) _cachedCombinedStats.Add(stat.Key, stat.Value);

            var finalMods = new Modifiers.Modifiers();
            finalMods.Add(LocalModifiers);

            foreach (var extMod in _externalModifiers) finalMods.Add(extMod);
            foreach (var buff in BuffStatuses) finalMods.Add(buff.modifiers);

            var keys = new List<(StatType Type, bool IsGlobal)>(_cachedCombinedStats.Keys);
            foreach (var key in keys) 
                _cachedCombinedStats[key] = finalMods.ApplyModByType(key.Type, _cachedCombinedStats[key], key.IsGlobal);

            IsDirty = false;
        }

        private void Update()
        {
            foreach (var stat in BaseStats)
            {
                //
            }
        }
    }
}
