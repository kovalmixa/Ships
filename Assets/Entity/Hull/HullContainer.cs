using Assets.Common;
using UnityEngine;

namespace Assets.DataContainers
{
    public class HullContainer : MonoBehaviour, IObject
    {
        public GeneralOptions General;

        public string Id { get; set; }

        public float MaxSpeed;

        public float Acceleration;

        public float RotationSpeed;

        public uint MaxHealth;

        public uint MaxEnergy;
    }
}