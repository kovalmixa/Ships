using Assets.Handlers.SceneHandlers;
using UnityEngine;
using Assets.Entity.Projectile;
using Assets.Scripts.Actions;

namespace Actions
{
    public class LaunchData
    {

    }
    
    public class FireProjectileAction : TemplateActionBase
    {
        public GameObject ProjectilePrefab;
        public Transform FirePosition;
        private ObjectPoolHandler poolHandler;

        private void Awake()
        {
            IsPassive = false;
            poolHandler = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            if (!CanActivate(interractionContext, targetPos) || poolHandler == null) return;
            Debug.Log("Pew");
            ProjectileController.Instance.Launch(interractionContext);
        }
    }
}
