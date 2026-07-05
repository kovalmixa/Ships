using Assets.Common.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class BuffStatController : MonoBehaviour, ICrud
    {
        public Dictionary<(StatType Type, StatLayer Layer), float> BaseStats { get; set; } = new();
        public Modifiers.Modifiers LocalModifiers { get; set; } = new();
        public Dictionary<(string buffId, string sourceId), (BuffStatus status, EntitySnapshot source)> ActiveBuffs { get; private set; } = new();
        public ILookup<string, BuffStatus> BuffsById => ActiveBuffs.Values.Select(v => v.status).ToLookup(b => b.BuffId);
       
        private bool _isDirty { get; set; } = true;
        private List<Modifiers.Modifiers> _externalModifiers = new();
        private Dictionary<(StatType Type, StatLayer Layer), float> _cachedCombinedStats = new();

        #region Modifiers

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
            foreach (var stat in BaseStats) _cachedCombinedStats.Add(stat.Key, stat.Value);

            var finalMods = new Modifiers.Modifiers();
            finalMods.Add(LocalModifiers);

            foreach (var extMod in _externalModifiers) finalMods.Add(extMod);
            foreach (var buff in ActiveBuffs.Values) finalMods.Add(buff.status.modifiers);
            var keys = new List<(StatType Type, StatLayer Layer)>(_cachedCombinedStats.Keys);
            foreach (var key in keys) 
                _cachedCombinedStats[key] = finalMods.ApplyModByType(key.Type, key.Layer, _cachedCombinedStats[key]);

            _isDirty = false;
        }

        #endregion

        #region Buffs
        public void AddBuff(BuffStatus newBuff, EntitySnapshot source)
        {
            if (newBuff == null) return;
            var key = (newBuff.BuffId, newBuff.SourceId);
            if (ActiveBuffs.TryGetValue(key, out var existing))
            {
                switch (newBuff.Policy)
                {
                    case BuffApplicationPolicy.Replace:
                    case BuffApplicationPolicy.UniquePerSource:
                        RemoveBuffInternal(existing.status);
                        break;
                    case BuffApplicationPolicy.Refresh:
                        existing.status.Duration = newBuff.Duration;
                        existing.status.onRefresh?.Invoke();
                        Destroy(newBuff);
                        _isDirty = true;
                        return;

                    case BuffApplicationPolicy.Stack:
                        break;
                }
            }
            ActiveBuffs[key] = (newBuff, source);
            newBuff.transform.SetParent(transform, false);
            _isDirty = true;
        }

        public bool RemoveBuff(string buffId, string sourceId = null)
        {
            bool removed = false;
            if (string.IsNullOrEmpty(sourceId))
            {
                foreach (var entry in ActiveBuffs.Where(kv => kv.Key.buffId == buffId).ToList())
                {
                    RemoveBuffInternal(entry.Value.status);
                    removed = true;
                }
            }
            else
            {
                var key = (buffId, sourceId);
                if (ActiveBuffs.TryGetValue(key, out var tuple))
                {
                    RemoveBuffInternal(tuple.status);
                    removed = true;
                }
            }

            if (removed) _isDirty = true;
            return removed;
        }

        private void RemoveBuffInternal(BuffStatus buff)
        {
            buff.onRemove?.Invoke();
            ActiveBuffs.Remove((buff.BuffId, buff.SourceId));
            Destroy(buff);
        }

        public void RemoveBuffBySource(string sourceId)
        {
            if (string.IsNullOrEmpty(sourceId)) return;

            var toRemove = ActiveBuffs.Where(kv => kv.Key.sourceId == sourceId).ToList();

            foreach (var entry in toRemove)
            {
                RemoveBuffInternal(entry.Value.status);
            }

            if (toRemove.Count > 0)
                _isDirty = true;
        }

        public void ClearAllBuffs()
        {
            foreach (var entry in ActiveBuffs.ToList())
                RemoveBuffInternal(entry.Value.status);

            _isDirty = true;
        }

        private void Update()
        {
            bool needsRebuild = false;
            foreach (var kv in ActiveBuffs.ToList())
            {
                var buff = kv.Value.status;
                if (buff == null || buff.Tick(new InterractionContext()))
                {
                    RemoveBuffInternal(buff);
                    needsRebuild = true;
                }
            }
            if (needsRebuild) _isDirty = true;
        }

        #endregion

        #region Crud
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
