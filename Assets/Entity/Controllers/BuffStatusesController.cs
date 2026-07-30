using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class BuffStatusesController
    {
        private bool _isDirty { get; set; } = true;
        public bool IsDirty => _isDirty;

        private Dictionary<(StatType Type, StatLayer Layer), float> _baseStats = new();
        public Dictionary<(StatType Type, StatLayer Layer), float> BaseStats => _baseStats;

        public Dictionary<(string buffId, string sourceId), (BuffStatus status, EntitySnapshot source)> ActiveBuffs { get; private set; } = new();
        public ILookup<string, BuffStatus> BuffsById => ActiveBuffs.Values.Select(v => v.status).ToLookup(b => b.Id);

        private GameObject _source;
        private StatModController _statMods;

        public BuffStatusesController(GameObject source, StatModController statMods)
        {
            _source = source;
            _statMods = statMods;
        }

        public void AddBuffs(InteractionContext context, params BuffStatus[] buffs)
        { foreach (var buff in buffs) AddBuff(buff, context.SourceSnapshot); }

        public void AddBuff(BuffStatus newBuff, EntitySnapshot source)
        {
            if (newBuff == null) return;
            var key = (newBuff.Id, newBuff.SourceId);
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
                        GameObject.Destroy(newBuff);
                        _isDirty = true;
                        return;

                    case BuffApplicationPolicy.Stack:
                        break;
                }
            }
            ActiveBuffs[key] = (newBuff, source);
            newBuff.transform.SetParent(_source.transform, false);
            _isDirty = true;
        }

        public void RemoveBuffs(params BuffStatus[] buffs)
        { foreach (var buff in buffs) RemoveBuff(buff.Id, buff.SourceId); }

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
            ActiveBuffs.Remove((buff.Id, buff.SourceId));
            GameObject.Destroy(buff);
        }

        public void RemoveBuffBySource(string sourceId)
        {
            if (string.IsNullOrEmpty(sourceId)) return;

            var toRemove = ActiveBuffs.Where(kv => kv.Key.sourceId == sourceId).ToList();
            foreach (var entry in toRemove) RemoveBuffInternal(entry.Value.status);
            if (toRemove.Count > 0) _isDirty = true;
        }

        public void ClearAllBuffs()
        {
            foreach (var entry in ActiveBuffs.ToList()) RemoveBuffInternal(entry.Value.status);
            _isDirty = true;
        }

        private void RebuildCachedStats()
        {
            var finalMods = new Modifiers.Modifiers();
            foreach (var buff in ActiveBuffs.Values) finalMods.Add(buff.status.modifiers);
            _statMods.RegisterExternalModifiers(finalMods);
            _isDirty = false;
        }

        public void Tick()
        {
            bool needsRebuild = false;
            foreach (var kv in ActiveBuffs.ToList())
            {
                var buff = kv.Value.status;
                if (buff == null || buff.Tick(null)) //add real interraction context of buff
                {
                    RemoveBuffInternal(buff);
                    needsRebuild = true;
                }
            }
            if (needsRebuild) _isDirty = true;
        }
    }
}
