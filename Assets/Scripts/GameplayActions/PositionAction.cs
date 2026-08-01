using Assets.Common;
using Assets.Scripts.Actions;
using System;
using UnityEngine;

namespace GameplayActions
{
    public class PositionDataSO : ActionDataSO
    {
        public Vector2 Position;
        public Vector2 Rotation;
    }

    public class PositionAction : GameplayAction<PositionDataSO>
    {
        protected override void ExecuteAction(InteractionContext context, PositionDataSO data, Vector2 targetPos)
        {
            throw new NotImplementedException();
        }

        protected override void ExecuteAction(InteractionContext context, PositionDataSO data, IInteractive target)
        {
            throw new NotImplementedException();
        }
    }
}
