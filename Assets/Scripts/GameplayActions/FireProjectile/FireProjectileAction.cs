using Assets.Common;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using UnityEngine;

namespace GameplayActions
{
    public class ProjectileData : ActionData
    {
        public string id;
        public Vector2 startPosition;
        public ProjectileType type;
        public DamageData damageData;
        public float speed;
        public float lifeTime;
        public bool isHoming;
        public bool isBallistic;
    }

    public class FireProjectileAction : GameplayAction<ProjectileData>
    {
        protected override void ExecuteAction(InteractionContext context, ProjectileData data, Vector2 targetPos)
        {
            if (context.SourceObject != null) data.startPosition = context.SourceObject.transform.TransformPoint(context.actionStartPosition);
            else data.startPosition = context.actionStartPosition;
            ProjectileController.Instance.Launch(context, data, targetPos);
        }
        protected override void ExecuteAction(InteractionContext context, ProjectileData data, IInteractive target) 
        {
        
        }
    }
}
