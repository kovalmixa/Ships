using Assets.Common.Interfaces;
using Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity
{
    [System.Serializable]
    public class EntityData : IDataContainer
    {
        public string hullId;
        public List<KeyValuePair<string, int>> equipmentIds;
        public Vector2 position;
    }

    public class EntitySnapshot
    {
        public global::Entity.Controllers.EntityController source;
        public EntityData entityData;

        public EntitySnapshot(global::Entity.Controllers.EntityController source, EntityData entityData)
        {
            this.source = source;
            this.entityData = entityData;
        }

        public string Id { get; internal set; }
    }
}
