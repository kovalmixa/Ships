using Assets.Common;
using UnityEngine;

namespace Assets.DataContainers
{
    public class HullContainer : MonoBehaviour, IObject
    {
        public GeneralOptions general;

        public string Id { get; set; }

        public float maxSpeed;

        public float acceleration;

        public float rotationSpeed;

        public uint maxHealth;

        public uint maxEnergy;
    }
}