using UnityEngine;

namespace Assets.DataContainers
{
    public class HullContainer : MonoBehaviour, IObject
    {
        public General General;

        public string Id { get; set; }

        public float MaxSpeed;

        public float Acceleration;

        public float RotationSpeed;

        public ActivationContainer[] OnActivate;
    }
}