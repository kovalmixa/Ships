using System;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    internal class PositionAction : MonoBehaviour, IGameAction
    {
        public bool IsPassive { get; set; }
        public void Execute(GameObject source, Vector3 targetPos)
        {
            throw new NotImplementedException();
        }
    }
}
