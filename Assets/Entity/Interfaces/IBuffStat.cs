using Assets.Entity.Modifiers;
using System.Collections.Generic;

namespace Assets.Entity
{
    public interface IBuffStat
    {
        public Dictionary<StatType, float> StatsDict { get; }
        public Modifiers.Modifiers Modifiers { get; set; }
        public List<BuffStatus> BuffStatuses { get; set; }
        public bool IsDirty { get; set; }

        void SetupStats();
        Dictionary<StatType, float> RebuildCachedStats();
        public float TryGetCurrentStat(StatType type);
    }
}
