using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Scripts.Actions.Projectile
{
    public class ProjectileInstance : MonoBehaviour
    {
        private readonly GameplayAction[] _onExplosionActions;
        protected Action OnExpload;

        protected ProjectileData data;
        protected InteractionContext context;

        protected Transform targetTransform;
        protected Vector2 targetPosition;
        protected Vector2 direction;

        protected float timer;

        #region Private/Default

        public void Tick(float deltaTime)
        {
            Move(deltaTime);
            CheckLifetime(deltaTime);
        }

        private void CheckLifetime(float deltaTime)
        {
            if (data.lifeTime != 0)
            {
                timer += deltaTime;
                if (timer >= data.lifeTime) Explode();
            }
        }

        private void IgnoreShooterCollision(bool ignore)
        {
            if (context?.SourceObject == null) return;

            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = context.SourceObject.GetComponent<Collider2D>();

            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, ignore);
        }

        #endregion

        #region Setup

        public virtual void Setup(
            InteractionContext interactionContext, 
            ProjectileData projectileDef, 
            Action onDeactivate,
            Transform targetTransform)
        {
            this.targetTransform = targetTransform;
            Setup(interactionContext, projectileDef, onDeactivate, targetTransform.position);
        }

        public virtual void Setup(InteractionContext interactionContext, ProjectileData data, Action onExpload, Vector2 targetPosition)
        {
            context = interactionContext;
            this.data = data;
            timer = 0f;
            this.targetPosition = targetPosition;
            direction = (this.targetPosition - this.data.startPosition).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(this.data.startPosition, Quaternion.Euler(0, 0, angle - 90f));

            IgnoreShooterCollision(true);
            SetupEffect();

            isExploded = false;
            OnExpload = onExpload;
            gameObject.SetActive(true);
        }

        private void SetupEffect()
        {

        }

        #endregion

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (context?.SourceObject != null && other.gameObject == context.SourceObject) return;
            Explode();
        }

        protected virtual void Move(float deltaTime)
        {
            if (data.isHoming && targetTransform != null)
            {
                Vector2 toTarget = (targetTransform.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, toTarget, deltaTime * 5f);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            transform.position += (Vector3)(direction * (data.speed * deltaTime));

            float distToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distToTarget <= 0.2f) Explode();
        }


        #region Explosion
        protected bool isExploded;

        public virtual void Explode()
        {
            if (isExploded) return;
            isExploded = true;
            Debug.Log("Exploaded");

            ExecuteExplosionActions();
            ReleaseToPool();
        }

        protected void ExecuteExplosionActions()
        {
            if (_onExplosionActions != null)
            {
                Vector3 explodePos = transform.position;
                var explosionAction = ActionProvider.Explosion;
                var dataController = context.ActionDataController;
                var data = dataController.GetActionData(explosionAction.GetType(), context);
                explosionAction.Execute(context, data, explodePos);

                foreach (var action in _onExplosionActions)
                {
                    data = dataController.GetActionData(action.GetType(), context);
                    action?.Execute(context, data, explodePos);
                }
            }
        }

        protected void ReleaseToPool()
        {
            OnExpload?.Invoke();
            OnExpload = null;
        }
        #endregion
    }
}