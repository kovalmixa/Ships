using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.WSA;

namespace Assets.Entity.Projectile
{
    public class ProjectileController : SingletonMonoBehaviour<ProjectileController>
    {
        private ObjectPoolHandler _projectilePool;

        public void Launch(InterractionContext<ProjectileDefinition> interractionContext)
        {
            var projectile = _projectilePool.Get();
            projectile.GetComponent<ProjectileInstance>().Setup(interractionContext);
        }

        protected override void Awake()
        {
            base.Awake();
            _projectilePool = ObjectPoolHandler.GetInstance(PoolType.Projectile);
        }

        private void Update()
        {
            var projectiles = _projectilePool.GetAllActive();
            foreach (var proj in projectiles)
            {
                
            }
        }
    }
}
