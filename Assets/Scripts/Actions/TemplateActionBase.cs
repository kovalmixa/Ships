using Assets.Common;
using Assets.Entity;
using UnityEngine;

namespace Actions
{
    public abstract class TemplateActionBase : MonoBehaviour
    {
        public string name;
        public float delay = 0;
        private float _lastActivationTime;

        public bool IsPassive { get; set; } = true;

        protected virtual void Awake()
        {
            name = gameObject.name;
        }

        public virtual void Execute(EntitySnapshot entitySnapshot, Vector3 targetPos) { }

        public virtual void Execute(EntitySnapshot entitySnapshot, IInteractive target) { }

        protected bool CanActivate(EntitySnapshot entitySnapshot, Vector3 targetPos)
        {
            if (delay == 0) return true;
            float time = Time.time;
            //Debug.Log($"Delta time: {time - lastActivationTime}");
            if (time - _lastActivationTime < delay) return false;
            _lastActivationTime = time;
            return true;
        }
    }
}
