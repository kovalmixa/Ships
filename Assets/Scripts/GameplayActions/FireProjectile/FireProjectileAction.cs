using Assets.Common;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using UnityEngine;

namespace GameplayActions
{
    public class ProjectileDataSO : ActionDataSO
    {
        public string id;
        public Vector2 startPosition;
        public ProjectileType type;
        public DamageDataSO damageData;
        public float speed;
        public float lifeTime;
        public bool isHoming;
        public bool isBallistic;
    }

    public class FireProjectileAction : GameplayAction<ProjectileDataSO>
    {
        protected override void ExecuteAction(InteractionContext context, ProjectileDataSO data, Vector2 targetPos)
        {
            ProjectileController.Instance.Launch(context, data, targetPos);
        }

        protected override void ExecuteAction(InteractionContext context, ProjectileDataSO data, IInteractive target) { }
    }
}
