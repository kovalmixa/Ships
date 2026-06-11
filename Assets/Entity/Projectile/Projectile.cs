using Actions;
using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.Entity.Interfaces;
using Assets.Entity.Projectile;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Entity.Projectile
{
    public class Projectile : MonoBehaviour, IActivation, IInteractive
    {
        public ProjectileContainer ProjectileContainer;
        public ActionBase[] OnExplosionActions;
        public ActionBase[] UpdateActions;

        private Transform target;
        private ActionContext action;
        private Vector2 direction;
        public Vector3? TargetPosition;
        private float timer;
        //private float distanceToTarget = 

        private ObjectPoolHandler objectPool;

        #region Start/Setup

        private void Start()
        {
            objectPool = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public void SetupByPrefab(Projectile prefab)
        {
            ProjectileContainer = prefab.ProjectileContainer;
            OnExplosionActions = prefab.OnExplosionActions;
            UpdateActions = prefab.UpdateActions;
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        }

        public void Launch(Vector2 dir, Vector3? targetPos = null, ActionContext action = null)
        {
            this.action = action;
            TargetPosition = targetPos;
            direction = dir;
            timer = 0f;

            if (action != null)
            {
                var projectileCollider = GetComponent<Collider2D>();
                var shooterCollider = action.Source.GetComponent<Collider2D>();
                if (projectileCollider != null && shooterCollider != null)
                    Physics2D.IgnoreCollision(projectileCollider, shooterCollider, true);
            }
            gameObject.SetActive(true);
        }

        #endregion

        #region Update/Activations

        private void Update()
        {
            if (ProjectileContainer.IsHoming && target != null)
            {
                Vector2 toTarget = (target.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, toTarget, Time.deltaTime * 5f);
            }
            transform.position += (Vector3)(direction * (ProjectileContainer.Speed * Time.deltaTime));
            timer += Time.deltaTime;
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
            if (timer > ProjectileContainer.LifeTime) Explode();
        }

        private void Explode()
        {
            Activate(transform.position, OnExplosionActions);
            Deactivate();
        }

        private void Deactivate()
        {
            if (objectPool != null) objectPool.Return(gameObject);
            else gameObject.SetActive(false);
            if (action == null) return;
            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = action.Source.GetComponent<Collider2D>();
            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, false);
        }

        public void Activate(Vector3 targetPos, ActionBase[] actions)
        {
            var actionContext = new ActionContext(gameObject, null);
            foreach (var activation in actions) activation.Execute(actionContext, targetPos);
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
        public void TakeDamage(ActionContext context, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(ActionContext context, Heal heal)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
