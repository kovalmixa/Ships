using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public class EsplosionDataSO : ActionDataSO
    {
        public uint range;

        public int[] layers;

        //[CanBeNull] public Dictionary<float, IScalableAction[]> ActionZones;

        public DamageDataSO damageData;

        [CanBeNull] public EffectDataSO visualData;

        public override void PopulateStatDict(Dictionary<StatType, float> targetDict)
        {
            throw new System.NotImplementedException();
        }

        public override Dictionary<StatType, float> ToStatTypeDict()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ExplosionAction : GameplayAction<EsplosionDataSO>
    {
        protected override void ExecuteAction(InteractionContext context, EsplosionDataSO data, Vector2 targetPos)
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

        protected override void ExecuteAction(InteractionContext context, EsplosionDataSO data, IInteractive target)
        {
            throw new System.NotImplementedException();
        }
    }
}
