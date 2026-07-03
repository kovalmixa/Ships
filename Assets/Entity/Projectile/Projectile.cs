using Actions;
using Assets.Common;
using Assets.Entity;
using Assets.Entity.Interfaces;
using Assets.Entity.Projectile;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Entity.Projectile
{
    public class Projectile : MonoBehaviour, IActivation, IInteractive
    {
        public ProjectileContainer projectileContainer;
        public TemplateActionBase[] onExplosionActions;
        public TemplateActionBase[] updateActions;

        private Transform _target;
        private EntitySnapshot _entitySnapshot;
        private Vector2 _direction;
        public Vector3? targetPosition;
        private float _timer;
        //private float distanceToTarget = 

        private ObjectPoolHandler _objectPool;

        #region Start/Setup

        public void SetupByPrefab(Projectile prefab)
        {
            projectileContainer = prefab.projectileContainer;
            onExplosionActions = prefab.onExplosionActions;
            updateActions = prefab.updateActions;
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        }

        public void Launch(Vector2 dir, Vector3? targetPos = null, EntitySnapshot entitySnapshot = null)
        {
            this._entitySnapshot = entitySnapshot;
            targetPosition = targetPos;
            _direction = dir;
            _timer = 0f;

            if (entitySnapshot != null)
            {
                var projectileCollider = GetComponent<Collider2D>();
                var shooterCollider = entitySnapshot.source.GetComponent<Collider2D>();
                if (projectileCollider != null && shooterCollider != null)
                    Physics2D.IgnoreCollision(projectileCollider, shooterCollider, true);
            }
            gameObject.SetActive(true);
        }

        #endregion

        #region Update/Activations

        private void Update()
        {
            if (projectileContainer.isHoming && _target != null)
            {
                Vector2 toTarget = (_target.position - transform.position).normalized;
                _direction = Vector2.Lerp(_direction, toTarget, Time.deltaTime * 5f);
            }
            transform.position += (Vector3)(_direction * (projectileContainer.speed * Time.deltaTime));
            _timer += Time.deltaTime;
            if (targetPosition.HasValue)
            {
                float distToTarget = Vector2.Distance(transform.position, targetPosition.Value);
                //DebugHandler.Instance.Log("DistanceLog", $"Distance = {distToTarget}", 0.1f);
                if (distToTarget <= 0.2f)
                {
                    Explode();
                    return;
                }
            }
            if (_timer > projectileContainer.lifeTime) Explode();
        }

        private void Explode()
        {
            Activate(transform.position, onExplosionActions);
            Deactivate();
        }

        private void Deactivate()
        {
            var objectPool = SceneNodesHandler.GetPoolHandler("ProjectilePool");
            if (_objectPool != null) _objectPool.Return(gameObject);
            else gameObject.SetActive(false);
            if (_entitySnapshot == null) return;
            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = _entitySnapshot.source.GetComponent<Collider2D>();
            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, false);
        }

        public void Activate(Vector3 targetPos, TemplateActionBase[] actions)
        {
            foreach (var activation in actions) activation.Execute(_entitySnapshot, targetPos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //если торпеда или абилки с жирными снарядами
            //if (other.gameObject == source)
            //{
            //    return;
            //}

            //if (other.CompareTag("Enemy"))
            //{
            //    Explode();
            //}
            //else
            //{
            //    Explode();
            //}
        }

        #endregion

        #region IInteractive
        public void TakeDamage(EntitySnapshot entitySnapshot, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(EntitySnapshot entitySnapshot, Heal heal)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
