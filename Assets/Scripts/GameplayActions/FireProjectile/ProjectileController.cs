using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileController : SingletonMonoBehaviour<ProjectileController>
    {
        private ObjectPoolHandler _projectilePool;

        public void Launch(InteractionContext interractionContext)
        {
            var projectile = _projectilePool.Get();
            var sourceTransform = interractionContext.SourceObject.transform;
            projectile.GetComponent<ProjectileInstance>().Setup(
                interractionContext, 
                () => _projectilePool.Return(projectile), 
                sourceTransform
                );
        }

        protected override void Awake()
        {
            base.Awake();
            _projectilePool = ObjectPoolHandler.GetInstance(PoolType.Projectile);
        }

        private void Update()
        {
            var projectiles = _projectilePool.GetAllActive()
                .Select(go => go.GetComponent<ProjectileInstance>())
                .Where(instance => instance != null)
                .ToArray();
            foreach (var proj in projectiles) proj.Tick(Time.deltaTime);
        }
    }
}
