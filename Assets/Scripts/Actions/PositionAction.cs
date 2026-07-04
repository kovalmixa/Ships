using Assets.Scripts.Actions;
using System;
using UnityEngine;

namespace Actions
{
    public class PositionAction : TemplateActionBase
    {
        [SerializeField] public Vector2 Position;
        [SerializeField] public Vector2 Rotation;
        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            throw new NotImplementedException();
        }
    }
}
