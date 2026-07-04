using Assets.Scripts.Actions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class ExplosionAction : TemplateActionBase
    {
        [SerializeField] public uint Range;

        [SerializeField] public int[] Layers;

        [SerializeField] [CanBeNull] public Dictionary<float, IScalableAction[]> ActionZones;

        [SerializeField] [CanBeNull] public VisualAction VisualAction;

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            VisualAction?.Execute(interractionContext, targetPos);

            var targetsToExecute = GetTargetsToExecuteInRange(targetPos, Range, Layers);

            foreach (var target in targetsToExecute)
                foreach (var zone in ActionZones)
                {
                    float rangeProp = Vector2.Distance(target.Value, targetPos) / Range;
                    if (zone.Key <= rangeProp)
                        foreach (var action in zone.Value)
                            action?.ScaleExecute(interractionContext, target.Key, 1 - rangeProp / zone.Key);
                }
        }
    }
}
