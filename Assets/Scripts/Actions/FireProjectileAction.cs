using Assets.Entity.Projectile;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class FireProjectileAction : ActionBase
    {
        public GameObject ProjectilePrefab;
        public Transform FirePosition;
        private ObjectPoolHandler _poolHandler;

        private void Awake()
        {
            IsPassive = false;
            _poolHandler = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos) || _poolHandler == null) return;
            Debug.Log("Pew");
            SetupProjectile(source, targetPos);
        }

        protected void SetupProjectile(GameObject source, Vector3 targetPos)
        {
            var prefabProjectile = ProjectilePrefab.GetComponent<Projectile>();
            if (prefabProjectile == null)
            {
                Debug.LogError($"Projectile prefab {prefabProjectile.name} doesnt have projectile component");
                return;
            }
            var pooledObj = _poolHandler.Get();
            var objProjectile = pooledObj.GetComponent<Projectile>();
            objProjectile.SetupByPrefab(prefabProjectile);
            pooledObj.transform.SetPositionAndRotation(FirePosition.position, Quaternion.identity);

            Vector3 direction = (targetPos - FirePosition.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pooledObj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
            objProjectile.Launch(direction, targetPos, source);
        }
    }
}
