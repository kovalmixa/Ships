using Entity.Controllers;
using UnityEngine;

namespace Assets.AI
{
    public interface IAiDriver : IDriver
    {
        public void SetMovementPoint(Transform target);
        public void SetTargetPoint(Transform target);
    }
}
