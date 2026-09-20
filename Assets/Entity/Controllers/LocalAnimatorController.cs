using UnityEngine;

namespace Assets.Entity.Controllers
{
    [RequireComponent(typeof(Animator))]
    public class LocalAnimatorController : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _actionHash = Animator.StringToHash("Action");
        private readonly int _takeDamageHash = Animator.StringToHash("TakeDamage");
        
        private readonly int _stateIdHash = Animator.StringToHash("StateID");
        private readonly int _animSpeed = Animator.StringToHash("ActAnimSpeed");
        private const float _minDuration = 0.2f;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void UpdateSpeed(float speed)
        {
            if (_animator != null) _animator.SetFloat(_speedHash, speed);
        }

        public void PlayAction(int actionId = 0, float animSpeed = 1f)
        {
            if (_animator == null) return;

            float baseClipLength = GetBaseClipLength();
            if (baseClipLength > 0f && _minDuration > 0f)
            {
                float maxAllowedSpeed = baseClipLength / _minDuration;
                animSpeed = Mathf.Min(animSpeed, maxAllowedSpeed);
            }

            _animator.SetFloat(_animSpeed, animSpeed);
            _animator.SetInteger(_stateIdHash, actionId);
            _animator.ResetTrigger(_actionHash);
            _animator.SetTrigger(_actionHash);
        }

        private float GetBaseClipLength()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (_animator.IsInTransition(0))
            {
                var nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
                return nextStateInfo.length;
            }
            return stateInfo.length;
        }

        public void PlayTakeDamage()
        {
            if (_animator != null) _animator.SetTrigger(_takeDamageHash);
        }
    }
}