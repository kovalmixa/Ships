using UnityEngine;

namespace Assets.Scripts.Actions
{
    public interface IGameAction
    {
        bool IsPassive { get; set; }
        void Execute(GameObject source, Vector3 targetPos);
    }
}
