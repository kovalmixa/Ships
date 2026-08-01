using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using UnityEngine;

namespace GameplayActions
{
    public struct ProjectileDefinition : IActionStruct
    {
        public string Id { get; set; }
        [field: SerializeField] public ProjectileType Type { get; set; }

        public Vector2? targetPosition;
        public Vector2 startPosition;

        public Damage damage;
        public float speed;
        public float penetration;
        public float critChance;

        public int lifeTime;
        public bool isHoming;
        public bool isBallistic;
    }

    public class FireProjectileAction : GameplayAction
    {
        public override void Execute(InteractionContext interractionContext, Vector3 targetPos)
        {
            //Debug.Log("Pew");
            ProjectileController.Instance.Launch(interractionContext);
        }
    }
}
