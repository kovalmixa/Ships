using Assets.Common;
using UnityEngine;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Decals;

namespace GameplayActions
{
    [System.Serializable]
    public class DecalData : ActionData
    {
        public Sprite decalSprite;
        public float disappearTime = -1f;
        public VfxRotationType rotationType = VfxRotationType.Default;
    }

    public class DecalAction : GameplayAction<DecalData>
    {
        protected override void ExecuteAction(InteractionContext context, DecalData data, Vector2 targetPos)
        {
            if (data == null || data.decalSprite == null) return;

            Quaternion rotation = GetRotation(context, data.rotationType, targetPos);
            DecalPoolController.Instance.SpawnDecal(data.decalSprite, targetPos, rotation, data.disappearTime);
        }

        protected override void ExecuteAction(InteractionContext context, DecalData data, IInteractive target)
        {
            if (data == null || data.decalSprite == null) return;

            if (target is MonoBehaviour monoBehaviour)
            {
                Vector3 pos = monoBehaviour.transform.position;
                Quaternion rotation = GetRotation(context, data.rotationType, pos);

                DecalPoolController.Instance.SpawnDecal(data.decalSprite, pos, rotation, data.disappearTime);
            }
        }

        private Quaternion GetRotation(InteractionContext context, VfxRotationType rotationType, Vector2 targetPos)
        {
            if (context?.SourceObject != null)
            {
                switch (rotationType)
                {
                    case VfxRotationType.MatchSource:
                        return context.SourceObject.transform.rotation;

                    case VfxRotationType.PointToTarget:
                        Vector2 sourcePos = context.SourceObject.transform.position;
                        Vector2 direction = (targetPos - sourcePos).normalized;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        return Quaternion.Euler(0, 0, angle);

                    case VfxRotationType.Default:
                    default:
                        return Quaternion.identity;
                }
            }

            return Quaternion.identity;
        }
    }
}