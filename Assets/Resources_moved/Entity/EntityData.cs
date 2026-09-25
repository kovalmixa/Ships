using Assets.Common.Interfaces;
using System.Collections.Generic;
using UnityEngine;

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
        [HideInInspector] public bool isPlayer;
        public string name;
        public string hullId;
        public List<EquipmentSlotData> equipmentSlots;

        public string fraction;
        public uint level;
    }
}
