using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EntityDataContainer
    {
        public string hullId;
        public List<KeyValuePair<string, int>> equipmentIds;
        public Vector2 position;
    }

    public class Snapshot
    {
        public EntityDataContainer entityData;
        public BuffStatus[] buffStatuses;

        public Snapshot(BuffStatus[] buffStatuses, EntityDataContainer entityData)
        {
            this.buffStatuses = buffStatuses;
            this.entityData = entityData;
        }
    }
}
