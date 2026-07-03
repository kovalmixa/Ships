using Assets.Common;
using Assets.Entity;
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

        public override void Execute(EntitySnapshot entitySnapshot, Vector3 targetPos)
        {
            VisualAction?.Execute(entitySnapshot, targetPos);

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
                            action?.ScaleExecute(entitySnapshot, target.Key, 1 - rangeProp / zone.Key);
                }
        }
    }
}
