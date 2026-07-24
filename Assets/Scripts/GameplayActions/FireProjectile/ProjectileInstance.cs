using Assets.Scripts.Actions;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileInstance : MonoBehaviour
    {
        private EffectAction _effectAction;
        private GameplayAction[] onExplosionActions;
        private ProjectileDefinition _definition;
        private Vector2 _direction;
        private InteractionContext _context;
        private Transform _targetTransform;
        private event Action _onDeactivate;
        private float _timer;
        
        #region Setup

        public void Setup(InteractionContext interactionContext, Action onDeactivate, Transform targetTransform = null)
        {
            if (interactionContext.ActionStruct is ProjectileDefinition projectileDef) _definition = projectileDef;
            else return;
            _context = interactionContext;
            _targetTransform = targetTransform;

            _timer = 0f;
            _onDeactivate = onDeactivate;

            if (_targetTransform != null)
                _direction = ((Vector2)_targetTransform.position - _definition.startPosition).normalized;
            else if (_definition.targetPosition.HasValue)
                _direction = (_definition.targetPosition.Value - _definition.startPosition).normalized;
            else _direction = transform.up;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(_definition.startPosition, Quaternion.Euler(0, 0, angle - 90f));

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
            if (_definition.isHoming && _targetTransform != null)
            {
                Vector2 toTarget = (_targetTransform.position - transform.position).normalized;
                _direction = Vector2.Lerp(_direction, toTarget, deltaTime * 5f);

                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            transform.position += (Vector3)(_direction * (_definition.speed * deltaTime));

            if (_definition.targetPosition.HasValue)
            {
                float distToTarget = Vector2.Distance(transform.position, _definition.targetPosition.Value);
                if (distToTarget <= 0.2f) Explode();
            }
        }

        private void CheckLifetime(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _definition.lifeTime) Explode();
        }

        public void Explode()
        {
            if (onExplosionActions != null)
            {
                Vector3 explodePos = transform.position;
                foreach (var action in onExplosionActions) action?.Execute(_context, explodePos);
            }
            _onDeactivate?.Invoke();
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