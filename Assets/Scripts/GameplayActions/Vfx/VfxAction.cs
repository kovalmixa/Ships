using Assets.Common;
using UnityEngine;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.VFX;

namespace GameplayActions
{
    public enum VfxRotationType
    {
        Default,            // Standard rotation (Quaternion.identity or target rotation)
        MatchSource,        // Rotation matching the source object (SourceObject)
        PointToTarget       // Rotation facing the target from the source
    }

    [System.Serializable]
    public class VfxData : ActionData
    {
        public VfxType type;
        public VfxRotationType rotationType = VfxRotationType.Default;
    }

    public class VfxAction : GameplayAction<VfxData>
    {
        protected override void ExecuteAction(InteractionContext context, VfxData data, Vector2 targetPos)
        {
            if (data == null || data.type == VfxType.None) return;

            Quaternion rotation = Quaternion.identity;

            if (context?.SourceObject != null)
            {
                switch (data.rotationType)
                {
                    case VfxRotationType.MatchSource:
                        rotation = context.SourceObject.transform.rotation;
                        break;

                    case VfxRotationType.PointToTarget:
                        Vector2 sourcePos = context.SourceObject.transform.position;
                        Vector2 direction = (targetPos - sourcePos).normalized;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        rotation = Quaternion.Euler(0, 0, angle);
                        break;

                    case VfxRotationType.Default:
                    default:
                        rotation = Quaternion.identity;
                        break;
                }
            }

            VfxController.Instance.PlayEffect(context, data.type, targetPos, rotation);
        }

        protected override void ExecuteAction(InteractionContext context, VfxData data, IInteractive target)
        {
            if (data == null || data.type == VfxType.None) return;

            if (target is MonoBehaviour monoBehaviour)
            {
                Vector3 pos = monoBehaviour.transform.position;
                Quaternion rot = monoBehaviour.transform.rotation;

                if (data.rotationType == VfxRotationType.MatchSource && context?.SourceObject != null)
                {
                    rot = context.SourceObject.transform.rotation;
                }
                else if (data.rotationType == VfxRotationType.PointToTarget && context?.SourceObject != null)
                {
                    Vector2 sourcePos = context.SourceObject.transform.position;
                    Vector2 direction = ((Vector2)pos - sourcePos).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    rot = Quaternion.Euler(0, 0, angle);
                }

                VfxController.Instance.PlayEffect(context, data.type, pos, rot);
            }
        }
    }
}