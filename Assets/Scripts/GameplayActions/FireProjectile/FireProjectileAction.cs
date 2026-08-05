using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using System.Collections.Generic;
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
            Debug.Log("pew");
            ProjectileController.Instance.Launch(context, data, targetPos);
        }

        protected override void ExecuteAction(InteractionContext context, ProjectileData data, IInteractive target) { }
    }
}
