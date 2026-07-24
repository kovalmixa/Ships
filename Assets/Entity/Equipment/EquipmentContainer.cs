using Assets.Common;
using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentContainer : MonoBehaviour
    {
        public GeneralOptions general;
        public string Id { get; set; }
        [field: SerializeField] public EquipmentType Type { get; private set; }
        [field: SerializeField] public ProjectileType ProjectileType { get; private set; }

        public StatOptions statOptions;
    }
}
