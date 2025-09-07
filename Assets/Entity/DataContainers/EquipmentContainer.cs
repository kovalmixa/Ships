using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class EquipmentContainer : MonoBehaviour, IObject
    {
        public General General;
        public string Id { get; set; }

        public float RotationSpeed;

        public ActivationContainer[] OnActivate;
    }
}
