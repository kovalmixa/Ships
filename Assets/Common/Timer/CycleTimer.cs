using Entity.Controllers.GenericController;
using Scripts;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Common
{
    public class CycleTimer
    {
        [SerializeField] private float delay;
        [SerializeField] private float duration;
        [SerializeField] private bool isActive = false;

        private float elapsedTime = 0f;
        private float elapsedDelay = 0f;
        private Action action;

        public CycleTimer() { }

        public CycleTimer(float delay, float duration, bool isActive, Action action)
        {
            this.delay = delay;
            this.duration = duration;
            this.isActive = isActive;
            this.action = action;
        }

        public void SetAction(Action action) => this.action = action 
            ?? throw new ArgumentNullException(nameof(action));

        public void Launch() => isActive = true;

        public void Stop() => isActive = false;

        public bool IsOver() => !isActive;

        private void Update()
        {
            if (!isActive || action == null) return;
            if (duration > 0f && elapsedTime >= duration)
            {
                isActive = false;
                return;
            }

            elapsedTime += Time.deltaTime;
            elapsedDelay += Time.deltaTime;

            if (elapsedDelay >= delay)
            {
                action();
                elapsedDelay = 0f;
            }
        }
    }
}
