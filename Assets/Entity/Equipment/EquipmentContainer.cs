using Assets.Common;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentContainer : MonoBehaviour, IObject
    {
        public GeneralOptions general;
        public string Id { get; set; }

        public float rotationSpeed;
    }
}
