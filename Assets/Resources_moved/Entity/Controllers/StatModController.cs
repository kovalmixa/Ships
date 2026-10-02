using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.StatMods;
using Entity.Controllers;

namespace Assets.Entity.Controllers
{
    public class StatModController : ICrud, IDirty
    {
        private readonly Dictionary<(StatType Type, StatLayer Layer), float> _baseStats = new();
        private readonly Dictionary<(StatType Type, StatLayer Layer), float> _cachedCombinedStats = new();

        private readonly List<ModUnit> _localModifiers = new();
        public IReadOnlyList<ModUnit> LocalModifiers => _localModifiers;

        private readonly List<IEnumerable<ModUnit>> _externalModifiers = new();
        private readonly EntityController _entityController;

        public StatModController() { }

        public StatModController(EntityController entityController, StatOptions statOptions)
        {
            _entityController = entityController;

            if (statOptions.stats != null)
            {
                _baseStats = statOptions.stats
                    .GroupBy(unit => (unit.Type, unit.StatLayer))
                    .ToDictionary(g => g.Key, g => g.Sum(unit => unit.Value));
            }

            if (statOptions.mods != null)
            {
            }

            MarkDirty();
        }

        #region Base Stats Management

        /// <summary>
        /// Устанавливает базовое значение характеристики для указанного типа и слоя.
        /// Если характеристики с такими типом и слоем нет, она создается.
        /// </summary>
        public void SetStatValue(StatType type, StatLayer layer, float value)
        {
            var key = (type, layer);

            // Записываем или обновляем значение базовой характеристики
            _baseStats[key] = value;

            MarkDirty();
        }

        #endregion

        #region Local modifiers

        public void AddLocalModifier(ModUnit mod)
        {
            if (mod == null) return;
            _localModifiers.Add(mod);
            MarkDirty();
        }

        public void AddLocalModifiers(IEnumerable<ModUnit> mods)
        {
            if (mods == null) return;
            _localModifiers.AddRange(mods);
            MarkDirty();
        }

        public bool RemoveLocalModifier(ModUnit mod)
        {
            if (mod == null) return false;
            bool removed = _localModifiers.Remove(mod);
            if (removed) MarkDirty();
            return removed;
        }

        public void ClearLocalModifiers()
        {
            if (_localModifiers.Count == 0) return;
            _localModifiers.Clear();
            MarkDirty();
        }

        #endregion

        #region External modifiers

        public void RegisterExternalModifiers(IEnumerable<ModUnit> mods)
        {
            if (mods == null || _externalModifiers.Contains(mods)) return;

            _externalModifiers.Add(mods);
            MarkDirty();
        }

        public void UnregisterExternalModifiers(IEnumerable<ModUnit> mods)
        {
            if (mods == null) return;
            if (_externalModifiers.Remove(mods)) MarkDirty();
        }

        #endregion

        public float GetStatValue(StatType type, StatLayer layer)
        {
            var key = (type, layer);

            if (_isDirty) RebuildCachedStats();
            if (_cachedCombinedStats.TryGetValue(key, out float value)) return value;
            return _baseStats.TryGetValue(key, out float baseValue) ? baseValue : 0f;
        }

        private void RebuildCachedStats()
        {
            _cachedCombinedStats.Clear();

            var activeModifiersGrouped = GetAllActiveModifiers()
                .GroupBy(m => (m.Type, m.StatLayer))
                .ToDictionary(g => g.Key, g => g.ToList());

            var allKeys = _baseStats.Keys.Union(activeModifiersGrouped.Keys);
            foreach (var key in allKeys)
            {
                _baseStats.TryGetValue(key, out float baseValue);
                activeModifiersGrouped.TryGetValue(key, out var modsForKey);

                _cachedCombinedStats[key] = CalculateFinalValue(baseValue, modsForKey);
            }

            _isDirty = false;
        }

        private IEnumerable<ModUnit> GetAllActiveModifiers()
        {
            foreach (var mod in _localModifiers)
                if (mod != null) yield return mod;

            foreach (var extGroup in _externalModifiers)
            {
                if (extGroup == null) continue;
                foreach (var mod in extGroup)
                    if (mod != null) yield return mod;
            }
        }

        private float CalculateFinalValue(float baseValue, List<ModUnit> modifiers)
        {
            if (modifiers == null || modifiers.Count == 0) return baseValue;

            float result = baseValue;
            float addition = 0f;
            float percentage = 0f;

            foreach (var mod in modifiers)
            {
                switch (mod.CalcType)
                {
                    case StatCalcType.Set:
                        result = mod.Value;
                        break;
                    case StatCalcType.Addition:
                        addition += mod.Value;
                        break;
                    case StatCalcType.Percentage:
                        percentage += mod.Value;
                        break;
                }
            }

            result += addition;
            result += result * (percentage / 100f);
            return result;
        }

        #region IDirty

        private bool _isDirty = false;
        public bool IsDirty => _isDirty;

        public void MarkDirty()
        {
            _isDirty = true;
            OnChange?.Invoke();
        }

        #endregion

        #region ICrud

        public event Action OnChange;
        public event Action OnDelete;
        public event Action OnInsert;

        #endregion
    }
}