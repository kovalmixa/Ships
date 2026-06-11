using Assets.Common;
using Assets.Common.ActionEffectStructs;
using UnityEngine;

namespace Actions
{
    public abstract class ActionBase : MonoBehaviour
    {
        public float Delay = 0;
        private float lastActivationTime;

        public bool IsPassive { get; set; } = true;

        public virtual void Execute(ActionContext context, Vector3 targetPos) { }

        public virtual void Execute(ActionContext context, IInteractive target) { }

        protected bool CanActivate(ActionContext context, Vector3 targetPos)
        {
            if (Delay == 0) return true;
            float time = Time.time;
            //Debug.Log($"Delta time: {time - lastActivationTime}");
            if (time - lastActivationTime < Delay) return false;
            lastActivationTime = time;
            return true;
        }
    }
}
