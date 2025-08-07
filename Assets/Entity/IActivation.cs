using Assets.Entity.DataContainers;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Assets.Entity
{
    public interface IActivation
    {
        public ActivationContainer[] Activations { get; set; }
        public void Activate(Vector3 targetPosition, string type = null);
    }
}
