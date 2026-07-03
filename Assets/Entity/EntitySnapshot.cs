using Entity.Controllers;
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

    public class EntitySnapshot
    {
        public EntityController source;
        public EntityDataContainer entityData;
        public BuffStatus[] buffStatuses;

        public EntitySnapshot(EntityController source, EntityDataContainer entityData, BuffStatus[] buffStatuses)
        {
            this.source = source;
            this.buffStatuses = buffStatuses;
            this.entityData = entityData;
        }
    }
}
