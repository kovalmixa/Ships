using Entity.Controllers;

namespace Assets.Entity
{
    public class EntitySnapshot
    {
        public EntityController source;
        public EntityData entityData;

        public EntitySnapshot(EntityController source, EntityData entityData)
        {
            this.source = source;
            this.entityData = entityData;
        }

        public string Id { get; internal set; }
    }
}
