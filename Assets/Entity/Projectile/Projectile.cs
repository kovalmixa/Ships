using System.Collections;
using Actions;
using Assets.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Handlers.SceneHandlers;
using Handlers.Console;
using UnityEngine;

namespace Entity.Projectile
{
    public class Projectile : MonoBehaviour, IActivation
    {
        public ProjectileContainer ProjectileContainer;
        public ActionBase[] OnExplosionActions;
        public ActionBase[] UpdateActions;

        private Transform _target;
        private GameObject _source;
        private Vector2 _direction;
        public Vector3? TargetPosition;
        private float _timer;
        //private float _distanceToTarget = 

        private ObjectPoolHandler _objectPool;

        #region Start/Setup

        private void Start()
        {
            _objectPool = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public void SetupByPrefab(Projectile prefab)
        {
            ProjectileContainer = prefab.ProjectileContainer;
            OnExplosionActions = prefab.OnExplosionActions;
            UpdateActions = prefab.UpdateActions;
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        }

        public void Launch(Vector2 dir, Vector3? targetPos = null, GameObject source = null)
        {
            _source = source;
            TargetPosition = targetPos;
            _direction = dir;
            _timer = 0f;

            if (_source != null)
            {
                var projectileCollider = GetComponent<Collider2D>();
                var shooterCollider = _source.GetComponent<Collider2D>();
                if (projectileCollider != null && shooterCollider != null)
                    Physics2D.IgnoreCollision(projectileCollider, shooterCollider, true);
            }
            gameObject.SetActive(true);
        }

        #endregion

        #region Update/Activations

        private void Update()
        {
            if (ProjectileContainer.IsHoming && _target != null)
            {
                Vector2 toTarget = (_target.position - transform.position).normalized;
                _direction = Vector2.Lerp(_direction, toTarget, Time.deltaTime * 5f);
            }
            transform.position += (Vector3)(_direction * (ProjectileContainer.Speed * Time.deltaTime));
            _timer += Time.deltaTime;
            if (TargetPosition.HasValue)
            {
                float distToTarget = Vector2.Distance(transform.position, TargetPosition.Value);
                //DebugHandler.Instance.Log("DistanceLog", $"Distance = {distToTarget}", 0.1f);
                if (distToTarget <= 0.2f)
                {
                    Explode();
                    return;
                }
            }
            if (_timer > ProjectileContainer.LifeTime) Explode();
        }

        private void Explode()
        {
            Activate(transform.position, OnExplosionActions);
            Deactivate();
        }

        private void Deactivate()
        {
            if (_objectPool != null) _objectPool.Return(gameObject);
            else gameObject.SetActive(false);
            if (_source == null) return;
            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = _source.GetComponent<Collider2D>();
            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, false);
        }

        public void Activate(Vector3 targetPos, ActionBase[] actions)
        {
            foreach (var activation in actions) activation.Execute(gameObject, targetPos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //если торпеда или абилки с жирными снарядами
            //if (other.gameObject == _source)
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
    }
}
