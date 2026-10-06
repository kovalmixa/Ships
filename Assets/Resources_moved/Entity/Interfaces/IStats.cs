using Assets.Common.Interfaces;
using Assets.Entity.StatMods;

namespace Assets.Entity
{
    public interface IStats
    {
        public float GetLifetimeStatValue(StatType type);
        public IUIData GetInitialData();
    }
}
