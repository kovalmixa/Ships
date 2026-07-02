using Entity.Controllers;
using UnityEngine;

namespace Assets.Entity.Controllers.AI
{
    public interface IAIEntityController : IEntityController
    {
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
