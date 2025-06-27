using Assets.Entity.DataContainers;
using Assets.Handlers;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Entity.Projectile
{
    public class Projectile : InGameObject
    {
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

        private Transform target;
        private GameObject shooter;
        private Vector2 direction;
        public Vector3? targetPosition;
        private float timer;

        private ProjectilePoolHandler projectilePool;

        private void Awake()
        {
            GetProjectilePool();
        }

        private void GetProjectilePool()
        {
            GameObject objectPool = GameObject.Find("ObjectPools");
            if (objectPool != null)
            {
                GameObject projectileObj = objectPool.transform.Find("ProjectilesPool")?.gameObject;
                if (projectileObj != null)
                    projectilePool = projectileObj.GetComponent<ProjectilePoolHandler>();
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
                if (distToTarget <= 0.1f) // порог можно подкорректировать
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
            Deactivate();
        }

        private void Deactivate()
        {
            if (projectilePool != null)
            {
                projectilePool.Return(gameObject);
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

        private void OnTriggerEnter2D(Collider2D other)
        {
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
