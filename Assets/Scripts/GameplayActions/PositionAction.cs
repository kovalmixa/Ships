using Assets.Common;
using Assets.Scripts.Actions;
using System;
using UnityEngine;

namespace GameplayActions
{
    [System.Serializable]
    public class PositionData : ActionData
    {
        public Vector2 Position;
        public Vector2 Rotation;
    }

    public class PositionAction : GameplayAction<PositionData>
    {
        protected override void ExecuteAction(InteractionContext context, PositionData data, Vector2 targetPos)
        {
            throw new NotImplementedException();
        }

        protected override void ExecuteAction(InteractionContext context, PositionData data, IInteractive target)
        {
            throw new NotImplementedException();
        }
    }
}
