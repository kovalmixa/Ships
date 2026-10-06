using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EquipmentSlotData
    {
        public string equipmentId;
        public int number;
    }

    [System.Serializable]
    public class EntityData
    {
        [HideInInspector] public bool isPlayer;
        [HideInInspector] public bool isDead;
        public string name;
        public string hullId;
        public List<EquipmentSlotData> equipmentSlots;

        public string fraction;
        public uint level;
    }
}
