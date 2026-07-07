using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileDefinition
    {
        public string Id { get; set; }
        [field: SerializeField] public ProjectileType Type { get; set; }

        public Vector2 targetPosition;
        public Vector2 startPosition;

        public Damage damage;
        public float speed;
        public float penetration;
        public float critChance;

        public int lifeTime;
        public bool isHoming;
        public bool isBallistic;
    }
}
