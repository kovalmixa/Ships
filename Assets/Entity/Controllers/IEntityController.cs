using Entity.Controllers.GenericController;
using UnityEngine;

namespace Entity.Controllers
{
    public interface IEntityController
    {
        public void UpdateControl(EntityController controller);
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
