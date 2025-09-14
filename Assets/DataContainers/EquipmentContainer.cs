using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.DataContainers
{
    public class EquipmentContainer : MonoBehaviour, IObject
    {
        public General General;
        public string Id { get; set; }

        public float RotationSpeed;

        public IGameAction[] OnActivate;
    }
}
