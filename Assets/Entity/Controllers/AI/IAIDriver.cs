using Entity.Controllers;
using UnityEngine;

namespace Assets.Entity.Controllers.AI
{
    public interface IAIDriver : IDriver
    {
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
