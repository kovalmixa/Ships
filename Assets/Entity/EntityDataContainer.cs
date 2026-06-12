using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EntityDataContainer
    {
        public int hullLayer;
        public string hullId;
        public List<KeyValuePair<string, int>> equipmentIds;
        public Vector2 position;
    }
}
