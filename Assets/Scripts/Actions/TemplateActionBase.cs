using Assets.Common;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public abstract class TemplateActionBase<T> : MonoBehaviour
    {
        public string name;
        public float delay = 0;
        private float _lastActivationTime;

        public bool IsPassive { get; set; } = true;

        protected virtual void Awake()
        {
            name = gameObject.name;
        }

        public virtual void Execute(InterractionContext<T> interractionContext, Vector3 targetPos) { }

        public virtual void Execute(InterractionContext<T> interractionContext, IInteractive target) { }

        #region Additional
        
        protected bool CanActivate(InterractionContext<T> interractionContext, Vector3 targetPos)
        {
            if (delay == 0) return true;
            float time = Time.time;
            //Debug.Log($"Delta time: {time - lastActivationTime}");
            if (time - _lastActivationTime < delay) return false;
            _lastActivationTime = time;
            return true;
        }

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
