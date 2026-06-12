using Assets.Common;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileContainer : MonoBehaviour, IObject
    {
        public string Id { get; set; }

        public float damage;
        public float speed;
        public float radius;
        public float penetration;
        public float critChance;

        public int lifeTime;
        public bool isHoming;
        public bool isBallistic;
    }
}
