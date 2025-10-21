using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Entity.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Actions
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
