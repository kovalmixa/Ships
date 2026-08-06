using Assets.Common.Interfaces;
using Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EntityDataContainer : IDataContainer
    {
        public string hullId;
        public List<KeyValuePair<string, int>> equipmentIds;
        public Vector2 position;
    }

    public class EntitySnapshot
    {
        public EntityController source;
        public EntityDataContainer entityData;

        public EntitySnapshot(EntityController source, EntityDataContainer entityData)
        {
            this.source = source;
            this.entityData = entityData;
        }

        public string Id { get; internal set; }
    }
}
