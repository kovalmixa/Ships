using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public class EsplosionData : ActionData
    {
        public uint range;
        public int[] layers;
        public DamageData damageData;
        [CanBeNull] public EffectData visualData;
        //[CanBeNull] public Dictionary<float, IScalableAction[]> ActionZones;
    }

    public class ExplosionAction : GameplayAction<EsplosionData>
    {
        protected override void ExecuteAction(InteractionContext context, EsplosionData data, Vector2 targetPos)
        {
            ActionProvider.Effect.Execute(context, data.visualData, targetPos);

            var targetsToExecute = GetTargetsToExecuteInRange(targetPos, data.range, data.layers);

            foreach (var target in targetsToExecute) { }
                //foreach (var zone in data.ActionZones)
                //{
                //    float rangeProp = Vector2.Distance(target.Value, targetPos) / data.range;
                //    if (zone.Key <= rangeProp)
                //        foreach (var action in zone.Value)
                //            action?.ScaleExecute(context, target.Key, 1 - rangeProp / zone.Key);
                //}
        }

        protected override void ExecuteAction(InteractionContext context, EsplosionData data, IInteractive target)
        {
            throw new System.NotImplementedException();
        }
    }
}
