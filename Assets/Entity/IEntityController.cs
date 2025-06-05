using UnityEngine;

namespace Assets.Entity
{
    public interface IEntityController
    {
        public void UpdateControl(Entity entity);
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
