using Assets.Scripts.Actions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    struct Esplosion : IActionStruct
    {
        [SerializeField] public uint Range;

        [SerializeField] public int[] Layers;

        [SerializeField][CanBeNull] public Dictionary<float, IScalableAction[]> ActionZones;

        [SerializeField][CanBeNull] public VisualAction VisualAction;
    }

    public class ExplosionAction : GameplayAction
    {
        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            var explosion = (Esplosion)interractionContext.ActionStruct;
            explosion.VisualAction?.Execute(interractionContext, targetPos);

            var targetsToExecute = GetTargetsToExecuteInRange(targetPos, explosion.Range, explosion.Layers);

            foreach (var target in targetsToExecute)
                foreach (var zone in explosion.ActionZones)
                {
                    float rangeProp = Vector2.Distance(target.Value, targetPos) / explosion.Range;
                    if (zone.Key <= rangeProp)
                        foreach (var action in zone.Value)
                            action?.ScaleExecute(interractionContext, target.Key, 1 - rangeProp / zone.Key);
                }
        }
    }
}
