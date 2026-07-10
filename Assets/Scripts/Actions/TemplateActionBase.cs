using Assets.Common;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public abstract class TemplateActionBase : MonoBehaviour
    {
        public virtual void Execute(InterractionContext interractionContext, Vector3 targetPos) { }

        public virtual void Execute(InterractionContext interractionContext, IInteractive target) { }

        #region Additional
        
        protected virtual Dictionary<IInteractive, Vector2> GetTargetsToExecuteInRange(
            Vector2 targetPos, float range, int[] layers)
        {
            var colliders = new List<Collider>();
            foreach (int layer in layers)
                colliders.AddRange(Physics.OverlapSphere(targetPos, range, layer));
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
            return targetsToExecute;
        }
        
        #endregion
    }
}
