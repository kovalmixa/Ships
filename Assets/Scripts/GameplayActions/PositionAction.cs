using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
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
