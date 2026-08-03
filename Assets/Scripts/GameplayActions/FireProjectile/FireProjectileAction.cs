using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using System.Collections.Generic;
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

        public override Dictionary<StatType, float> ToStatTypeDict()
        {
            var dict = new Dictionary<StatType, float>();
            PopulateStatDict(dict);
            return dict;
        }

        public override void PopulateStatDict(Dictionary<StatType, float> targetDict)
        {
            targetDict.Clear();
            targetDict[StatType.PrSpeed] = speed;
            targetDict[StatType.PrLifeTime] = lifeTime;
            targetDict[StatType.PrIsHoming] = isHoming ? 1f : 0f;
            targetDict[StatType.PrMoveType] = isBallistic ? 1f : 0f;
            targetDict[StatType.PrType] = (float)type;
        }
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
