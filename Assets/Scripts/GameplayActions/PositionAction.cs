using Assets.Scripts.Actions;
using System;
using UnityEngine;

namespace GameplayActions
{
    public class PositionAction : GameplayAction
    {
        [SerializeField] public Vector2 Position;
        [SerializeField] public Vector2 Rotation;
        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
        }
    }
}
