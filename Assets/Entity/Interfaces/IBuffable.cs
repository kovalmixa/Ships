using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;

namespace Assets.Entity
{
    public interface IBuffable
    {
        public StatModController BuffController { get; }
        public Dictionary<StatType, float> LifetimeStats { get; }

        public void ResetLifetimeStats();

        public float GetLifetimeStat(StatType type);
    }
}
