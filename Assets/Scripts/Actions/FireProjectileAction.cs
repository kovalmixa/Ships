using Assets.Handlers.SceneHandlers;
using Entity.Projectile;
using UnityEngine;
using Assets.Scripts.Actions;
using Assets.Entity.Projectile;

namespace Actions
{
    public class LaunchData
    {

    }
    
    public class FireProjectileAction : TemplateActionBase<ProjectileDefinition>
    {
        public GameObject ProjectilePrefab;
        public Transform FirePosition;
        private ObjectPoolHandler poolHandler;

        private void Awake()
        {
            IsPassive = false;
            poolHandler = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public override void Execute(InterractionContext<ProjectileDefinition> interractionContext, Vector3 targetPos)
        {
            if (!CanActivate(interractionContext, targetPos) || poolHandler == null) return;
            Debug.Log("Pew");
            ProjectileController.Instance.Launch(interractionContext);
        }
    }
}
