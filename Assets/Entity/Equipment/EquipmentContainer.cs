using Assets.Common;
using Assets.Entity.Hull;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentContainer : MonoBehaviour, IObject
    {
        public GeneralOptions General;
        public string Id { get; set; }

        public float RotationSpeed;
    }
}
