using Assets.Scripts.Actions;
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
    }
}
