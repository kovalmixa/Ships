using Assets.Entity.Modifiers;
using System.Collections.Generic;

namespace Assets.Entity
{
    public interface IStats
    {
        public Dictionary<StatType, float> LifetimeStats { get; }

        public void ResetLifetimeStats();

        public float GetLifetimeStat(StatType type);
    }
}
