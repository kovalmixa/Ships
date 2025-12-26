using UnityEngine;

namespace Actions
{
    public abstract class ActionBase : MonoBehaviour
    {
        public float Delay = 0;
        private float _lastActivationTime;

        public bool IsPassive { get; set; } = true;

        public abstract void Execute(GameObject source, Vector3 targetPos);

        protected bool CanActivate(GameObject source, Vector3 targetPos)
        {
            if (Delay == 0) return true;
            float time = Time.time;
            //Debug.Log($"Delta time: {time - _lastActivationTime}");
            if (time - _lastActivationTime < Delay) return false;
            _lastActivationTime = time;
            return true;
        }
    }
}
