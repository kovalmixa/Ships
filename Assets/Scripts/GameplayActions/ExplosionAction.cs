using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    [System.Serializable]
    public class ExplosionData : ActionData
    {
        public DamageData damageData;
        public VfxData vfxData;
    }

    public class ExplosionAction : GameplayAction<ExplosionData>
    {
        protected override void ExecuteAction(InteractionContext context, ExplosionData data, Vector2 targetPos)
        {
            ActionProvider.Effect.Execute(context, data.vfxData, targetPos);
            ActionProvider.Damage.Execute(context, data.damageData, targetPos);
        }

        protected override void ExecuteAction(InteractionContext context, ExplosionData data, IInteractive target)
        {
            ActionProvider.Effect.Execute(context, data.vfxData, target);
            ActionProvider.Damage.Execute(context, data.damageData, target);
        }
    }
}
