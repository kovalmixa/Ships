using System;
using System.Collections.Generic;

namespace Assets.Entity.DataContainers
{
    [System.Serializable]
    public class EntityDataContainer
    {
        public string HullId;
        public List<KeyValuePair<string, int>> EquipmentIds;
    }
}
