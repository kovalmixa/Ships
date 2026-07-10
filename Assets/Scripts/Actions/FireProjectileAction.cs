using Assets.Handlers.SceneHandlers;
using UnityEngine;
using Assets.Entity.Projectile;
using Assets.Scripts.Actions;

namespace Actions
{
    public class FireProjectileAction : TemplateActionBase
    {
        private ObjectPoolHandler poolHandler;

        private void Awake()
        {
            poolHandler = SceneNodesHandler.GetPoolHandler("ProjectilePool");
        }

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            Debug.Log("Pew");
            ProjectileController.Instance.Launch(interractionContext);
        }
    }
}
