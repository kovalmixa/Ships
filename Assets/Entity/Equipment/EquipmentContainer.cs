using Assets.Common;
using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentContainer : MonoBehaviour
    {
        public GeneralOptions general;
        public string Id { get; set; }
        public EquipmentSubType Type { get; }

        public StatOptions statOptions;
    }
}
