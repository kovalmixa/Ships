using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Scripts.Actions.Projectile
{
    public class ProjectileInstance : MonoBehaviour
    {
        private readonly EffectAction _effectAction;
        private readonly GameplayAction[] _onExplosionActions;
        private event Action OnDeactivate;

        private readonly ProjectileDataSO _data;
        private InteractionContext _context;

        private Transform _targetTransform;
        private Vector2 _targetPosition;
        private Vector2 _direction;

        private float _timer;

        #region Setup

        public void Setup(
            InteractionContext interactionContext, 
            ProjectileDataSO projectileDef, 
            Action onDeactivate,
            Transform targetTransform)
        {
            _targetTransform = targetTransform;
            Setup(interactionContext, projectileDef, onDeactivate, targetTransform.position);
        }

        public void Setup(
            InteractionContext interactionContext,
            ProjectileDataSO projectileDef,
            Action onDeactivate,
            Vector2 targetPosition)
        {
            _context = interactionContext;

            _timer = 0f;
            OnDeactivate = onDeactivate;

            //if (_targetTransform != null)
            //    _direction = ((Vector2)_targetTransform.position - _definition.startPosition).normalized;
            _targetPosition = targetPosition;
            _direction = (_targetPosition - _data.startPosition).normalized;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(_data.startPosition, Quaternion.Euler(0, 0, angle - 90f));

            IgnoreShooterCollision(true);
            SetupEffect();

            gameObject.SetActive(true);
        }

        private void SetupEffect()
        {

        }

        #endregion

        public void Tick(float deltaTime)
        {
            Move(deltaTime);
            CheckLifetime(deltaTime);
        }

        private void Move(float deltaTime)
        {
            if (_data.isHoming && _targetTransform != null)
            {
                Vector2 toTarget = (_targetTransform.position - transform.position).normalized;
                _direction = Vector2.Lerp(_direction, toTarget, deltaTime * 5f);

                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            transform.position += (Vector3)(_direction * (_data.speed * deltaTime));

            float distToTarget = Vector2.Distance(transform.position, _targetPosition);
            if (distToTarget <= 0.2f) Explode();
        }

        private void CheckLifetime(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _data.lifeTime) Explode();
        }

        public void Explode()
        {
            if (_onExplosionActions != null)
            {
                Vector3 explodePos = transform.position;
                var explosionAction = ActionProvider.Explosion;
                var data = ActionDataFactory.CreateDynamicData(explosionAction.GetType(), _context);
                explosionAction.Execute(_context, data, explodePos);

                foreach (var action in _onExplosionActions)
                {
                    data = ActionDataFactory.CreateDynamicData(action.GetType(), _context);
                    action?.Execute(_context, data, explodePos);
                }
            }
            OnDeactivate?.Invoke();
        }

        private void IgnoreShooterCollision(bool ignore)
        {
            if (_context?.SourceObject == null) return;

            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = _context.SourceObject.GetComponent<Collider2D>();

            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, ignore);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_context?.SourceObject != null && other.gameObject == _context.SourceObject) return;
            Explode();
        }
    }
}