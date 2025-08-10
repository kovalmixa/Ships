using System;
using Assets.Entity.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using Assets.InGameMarkers.Actions;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Entity.Projectile
{
    public class Projectile : InGameObject, IActivation
    {
        private Activator _activator;

        private ProjectileContainer _projectileContainer;
        public ProjectileContainer ProjectileContainer
        {
            get => _projectileContainer;
            set
            {
                _projectileContainer = value;
                string[] texturePaths = _projectileContainer.Graphics.Textures;
                IsTrigger = true;
                StartCoroutine(SetupLayersCoroutine(texturePaths));
            }
        }
        public ActivationContainer[] Activations
        {
            get => ProjectileContainer.OnActivate;
            set => ProjectileContainer.OnActivate = value;
        }

        private Transform target;
        private GameObject shooter;
        private Vector2 direction;
        public Vector3? targetPosition;
        private float timer;

        private ObjectPoolHandler _objectPool;

        private void Awake()
        {
            GetProjectilePool();
            _activator = gameObject.AddComponent<Activator>();
        }

        private void GetProjectilePool()
        {
            GameObject objectPool = GameObject.Find("ObjectPools");
            if (objectPool != null)
            {
                GameObject projectileObj = objectPool.transform.Find("ProjectilesPool")?.gameObject;
                if (projectileObj != null)
                    _objectPool = projectileObj.GetComponent<ObjectPoolHandler>();
            }
        }

        public void Launch(Vector2 dir, Transform homingTarget = null, Vector3? targetPos = null, GameObject shooter = null)
        {
            this.shooter = shooter;
            direction = dir.normalized;
            target = homingTarget;
            targetPosition = targetPos;
            timer = 0f;

            if (this.shooter != null)
            {
                var projectileCollider = GetComponent<Collider2D>();
                var shooterCollider = this.shooter.GetComponent<Collider2D>();
                if (projectileCollider != null && shooterCollider != null)
                    Physics2D.IgnoreCollision(projectileCollider, shooterCollider, true);
            }
            _activator.SetActivations(Activations);
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (ProjectileContainer.IsHoming && target != null)
            {
                Vector2 toTarget = (target.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, toTarget, Time.deltaTime * 5f);
            }

            transform.position += (Vector3)(direction * ProjectileContainer.Speed * Time.deltaTime);
            timer += Time.deltaTime;

            if (targetPosition.HasValue)
            {
                float distToTarget = Vector3.Distance(transform.position, targetPosition.Value);
                if (distToTarget <= 0.5f) // порог можно подкорректировать
                {
                    Explode();
                    return;
                }
            }

            if (timer > ProjectileContainer.LifeTime)
                Deactivate();
        }

        private void Explode()
        {
            Activate(transform.position);
            Deactivate();
        }

        private void Deactivate()
        {
            if (_objectPool != null)
            {
                _objectPool.Return(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
            if (shooter != null)
            {
                var projectileCollider = GetComponent<Collider2D>();
                var shooterCollider = shooter.GetComponent<Collider2D>();
                if (projectileCollider != null && shooterCollider != null)
                    Physics2D.IgnoreCollision(projectileCollider, shooterCollider, false);
            }
        }

        public void Activate(Vector3 targetPosition, string type = null) => _activator.TryActivate(targetPosition, type);

        private void OnTriggerEnter2D(Collider2D other)
        {
            //если торпеда или абилки с жирными снарядами
            //if (other.gameObject == shooter)
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
    }
}
