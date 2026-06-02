using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EntityDataContainer
    {
        public int HullLayer;
        public string HullId;
        public List<KeyValuePair<string, int>> EquipmentIds;
        public Vector2 Position;
    }
}
