using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public class PositionDataSO : ActionDataSO
    {
        public Vector2 Position;
        public Vector2 Rotation;

        public override void PopulateStatDict(Dictionary<StatType, float> targetDict)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<StatType, float> ToStatTypeDict()
        {
            throw new NotImplementedException();
        }
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
