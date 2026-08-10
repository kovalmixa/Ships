using Assets.Common;
using UnityEngine;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.VFX;

namespace GameplayActions
{
    [System.Serializable]
    public class VfxData : ActionData
    {
        public VfxType type;
        public bool matchSourceRotation = false;
        public bool pointToTarget = false;
    }

    public class VfxAction : GameplayAction<VfxData>
    {
        protected override void ExecuteAction(InteractionContext context, VfxData data, Vector2 targetPos)
        {
            if (data == null || data.type == VfxType.None) return;
            Quaternion rotation = Quaternion.identity;
            if (context?.SourceObject != null)
            {
                if (data.matchSourceRotation) rotation = context.SourceObject.transform.rotation;
                else if (data.pointToTarget)
                {
                    Vector2 sourcePos = context.SourceObject.transform.position;
                    Vector2 direction = (targetPos - sourcePos).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    rotation = Quaternion.Euler(0, 0, angle);
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
                if (data.matchSourceRotation && context?.SourceObject != null)
                    rot = context.SourceObject.transform.rotation;
                VfxController.Instance.PlayEffect(context, data.type, pos, rot);
            }
        }
    }
}
