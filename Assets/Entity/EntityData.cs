using Assets.Common.Interfaces;
using System.Collections.Generic;

namespace Assets.Entity
{
    [System.Serializable]
    public class EquipmentSlotData : IDataContainer
    {
        public string equipmentId;
        public int number;
    }

    [System.Serializable]
    public class EntityData : IDataContainer
    {
        public string hullId;
        public List<EquipmentSlotData> equipmentSlots;
    }
}
