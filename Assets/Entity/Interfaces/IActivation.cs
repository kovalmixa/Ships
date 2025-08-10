using Assets.Entity.DataContainers;
using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IActivation
    {
        public ActivationContainer[] Activations { get; set; }
        public void Activate(Vector3 targetPosition, string type = null);
    }
}
