using Assets.Common;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileContainer : MonoBehaviour, IObject
    {
        public string Id { get; set; }
        public int LifeTime;
        public int Speed;
        public bool IsHoming;
        public bool IsBallistic;
    }
}
