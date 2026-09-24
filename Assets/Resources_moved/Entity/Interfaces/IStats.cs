using Assets.Common.Interfaces;
using Assets.Entity.Modifiers;

namespace Assets.Entity
{
    public interface IStats
    {
        public float GetLifetimeStat(StatType type);
        public IDataContainer GetInitialData();
    }
}
