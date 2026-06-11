using Assets.Common;
using Assets.Common.ActionEffectStructs;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class ExplosionAction : ActionBase
    {
        [SerializeField] public uint Range;

        [SerializeField] public int[] Layers;

        [SerializeField] [CanBeNull] private Dictionary<float, IScalableAction[]> ActionZones;

        [SerializeField] [CanBeNull] private VisualAction visualAction;

        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            visualAction?.Execute(context, targetPos);

            var colliders = new List<Collider>();
            foreach(int layer in Layers)
                colliders.AddRange(Physics.OverlapSphere(targetPos, Range, layer));

            //make the same for tiles
            //

            var targetsToExecute = new Dictionary<IInteractive, Vector2>();
            foreach (var collider in colliders)
            {
                var target = collider.GetComponent<IInteractive>();
                if (target != null) continue;
                var transform = collider.GetComponent<Transform>();
                if (transform != null) targetsToExecute.Add(target, transform.position);
            }

            foreach (var target in targetsToExecute)
                foreach (var zone in ActionZones)
                {
                    float rangeProp = Vector2.Distance(target.Value, targetPos) / Range;
                    if (zone.Key <= rangeProp)
                        foreach (var action in zone.Value)
                            action?.ScaleExecute(context, target.Key, 1 - rangeProp / zone.Key);
                }
        }
    }
}
