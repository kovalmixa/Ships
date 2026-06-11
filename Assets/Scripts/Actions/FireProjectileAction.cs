using Assets.Common.ActionEffectStructs;
using Assets.Handlers.SceneHandlers;
using Entity.Projectile;
using UnityEngine;

namespace Actions
{
    public class FireProjectileAction : ActionBase
    {
        public GameObject ProjectilePrefab;
        public Transform FirePosition;
        private ObjectPoolHandler poolHandler;

        private void Awake()
        {
            IsPassive = false;
            poolHandler = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            if (!CanActivate(context, targetPos) || poolHandler == null) return;
            Debug.Log("Pew");
            SetupProjectile(context, targetPos);
        }

        protected void SetupProjectile(ActionContext context, Vector3 targetPos)
        {
            var prefabProjectile = ProjectilePrefab.GetComponent<Projectile>();
            if (prefabProjectile == null)
            {
                Debug.LogError($"Projectile prefab {prefabProjectile.name} doesnt have projectile component");
                return;
            }
            var pooledObj = poolHandler.Get();
            var objProjectile = pooledObj.GetComponent<Projectile>();
            objProjectile.SetupByPrefab(prefabProjectile);
            pooledObj.transform.SetPositionAndRotation(FirePosition.position, Quaternion.identity);

            Vector3 direction = (targetPos - FirePosition.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pooledObj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
            objProjectile.Launch(direction, targetPos, context);
        }
    }
}
