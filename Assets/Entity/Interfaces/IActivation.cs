using Actions;
using Assets.DataContainers;
using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IActivation
    {
        public void Activate(Vector3 targetPos, ActionBase[] actions);
    }
}
