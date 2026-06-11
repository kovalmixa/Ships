using Assets.Common.ActionEffectStructs;
using System;
using UnityEngine;

namespace Actions
{
    public class PositionAction : ActionBase
    {
        [SerializeField] public Vector2 Position;
        [SerializeField] public Vector2 Rotation;
        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            throw new NotImplementedException();
        }
    }
}
