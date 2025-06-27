using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class Physics
    {
        public int Mass { get; set; }
        public int RotationSpeed { get; set; }
        public string CollisionType { get; set; }
        public bool HasCollision { get; set; }
    }
}
