using System;
using System.Collections.Generic;
using Assets.Entity.Modifiers;
using Entity.Controllers;

namespace Assets.Entity.Controllers
{
    public class EntityStatsAggregator : IDirty
    {
        private readonly EntityController _entity;
        private readonly Dictionary<StatType, float> _cachedStats = new();
        private bool _isDirty = true;

        public bool IsDirty => _isDirty;

        public EntityStatsAggregator(EntityController entity) => _entity = entity;

        public void MarkDirty() => _isDirty = true;

        public float GetStat(StatType type)
        {
            if (_isDirty) RebuildCachedStats();
            return _cachedStats.TryGetValue(type, out float value) ? value : 0f;
        }

        private void RebuildCachedStats()
        {
            _cachedStats.Clear();

            var hull = _entity.hull;
            if (hull == null)
            {
                _isDirty = false;
                return;
            }

            var sources = new List<IStats> { hull };

            if (hull.equipments != null)
                foreach (var eq in hull.equipments) if (eq is IStats statsSource) sources.Add(statsSource);

            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                float aggregatedValue = CalculateValueForType(statType, sources);
                if (_entity.StatModController != null)
                {
                    // its a bullshit

                    float entityMod = _entity.StatModController.GetStat(statType, StatLayer.Global);
                    aggregatedValue += entityMod;
                }

                if (MathF.Abs(aggregatedValue) > 0.0001f) _cachedStats[statType] = aggregatedValue;
            }

            _isDirty = false;
        }

        private float CalculateValueForType(StatType type, List<IStats> sources)
        {
            // its a bullshit
            bool isMaxRule = type == StatType.MaxRange || type == StatType.MaxMoveSpeed;

            float result = isMaxRule ? float.MinValue : 0f;
            bool hasValue = false;

            foreach (var source in sources)
            {
                float val = source.GetLifetimeStat(type);
                if (val == 0f && isMaxRule) continue;

                if (isMaxRule)
                {
                    if (val > result) result = val;
                    hasValue = true;
                }
                else
                {
                    result += val;
                    hasValue = true;
                }
            }

            if (isMaxRule && !hasValue) return 0f;
            return result;
        }
    }
}