
using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class HullContainer : MonoBehaviour, IObject
    {
        public General General;

        public string Id { get; set; }

        public ActivationContainer[] OnActivate;
    }
}