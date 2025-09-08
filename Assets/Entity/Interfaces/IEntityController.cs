using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IEntityController
    {
        public void UpdateControl(EntityController controller);
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
