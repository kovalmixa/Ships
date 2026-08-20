using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    [System.Serializable]
    public class PositionData : ActionData
    {
        public Vector2 offset;
        public Quaternion? rotationOffset;
        public bool isRelative = true;
    }

    public class PositionAction : GameplayAction<PositionData>
    {
        protected override void ExecuteAction(InteractionContext context, PositionData data, Vector2 targetPos)
        {
            if (context.SourceObject == null) return;
            Transform targetTransform = context.SourceObject.transform;
            ApplyPositionAndRotation(targetTransform, data, targetPos);
        }

        protected override void ExecuteAction(InteractionContext context, PositionData data, IInteractive target)
        {
            if (target?.GameObject == null) return;
            Transform targetTransform = target.GameObject.transform;
            ApplyPositionAndRotation(targetTransform, data, targetTransform.position);
        }

        private void ApplyPositionAndRotation(Transform transformToMove, PositionData data, Vector2 basePosition)
        {
            Vector2 finalPos = data.isRelative
                ? (Vector2)transformToMove.TransformPoint(data.offset)
                : basePosition + data.offset;
            transformToMove.position = finalPos;

            if (data.rotationOffset.HasValue)
            {
                transformToMove.rotation = data.isRelative
                    ? transformToMove.rotation * data.rotationOffset.Value
                    : data.rotationOffset.Value;
            }
        }
    }
}