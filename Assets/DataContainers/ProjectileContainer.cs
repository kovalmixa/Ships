using UnityEngine;

namespace Assets.DataContainers
{
    public class ProjectileContainer : MonoBehaviour, IObject
    {
        public string Id { get; set; }
        public int LifeTime;
        public int Speed;
        public bool IsHoming;
        public bool IsBallistic;

        public ActivationContainer[] OnActivate;
        public string Explosion;
    }
}
