using System.Collections.Generic;
using System.Linq;
using Assets.Common.Interfaces;
using Assets.Entity.BuffStatuses;
using Assets.Entity.Modifiers;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class BuffStatController : MonoBehaviour, ICrud
    {
        public Dictionary<(StatType Type, bool IsGlobal), float> BaseStats { get; set; } = new();
        public Modifiers.Modifiers LocalModifiers { get; set; } = new();
        public List<(BuffStatus status, EntitySnapshot source)> BuffStatuses { get; set; } = new();
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
            foreach (var buff in BuffStatuses) finalMods.Add(buff.status.modifiers);

            var keys = new List<(StatType Type, bool IsGlobal)>(_cachedCombinedStats.Keys);
            foreach (var key in keys) 
                _cachedCombinedStats[key] = finalMods.ApplyModByType(key.Type, _cachedCombinedStats[key], key.IsGlobal);

            IsDirty = false;
        }

        private void Update()
        {
            bool needsRebuild = false;
            StatusContext context = new StatusContext();
            for (int i = BuffStatuses.Count - 1; i >= 0; i--)
            {
                var tuple = BuffStatuses[i];
                var buff = tuple.status;

                if (buff == null)
                {
                    BuffStatuses.RemoveAt(i);
                    needsRebuild = true;
                    continue;
                }

                bool isExpired = buff.Tick(context);

                if (isExpired)
                {
                    buff.onRemove?.Invoke();
                    Destroy(buff);
                    BuffStatuses.RemoveAt(i);
                    needsRebuild = true;
                }
            }
            if (needsRebuild) IsDirty = true;
        }

        #region
        public void OnUpdate()
        {
            RebuildCachedStats();
        }

        public void OnDelete()
        {
            throw new System.NotImplementedException();
        }

        public void OnInsert()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
