using Entity.Controllers;
using UnityEngine;

namespace Assets.AI
{
    public interface IAiDriver : IDriver
    {
        public Transform MovePoint { get; set; }
        public Transform TargetPoint { get; set; }
    }
}
