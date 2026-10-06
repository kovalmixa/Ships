using Assets.Handlers.Enums;
using Assets.Entity.Parts;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    [CreateAssetMenu(fileName = "NewEquipmentData", menuName = "Configs/Equipment Data")]
    public class EquipmentDataSO : EntityPartDataSO
    {
        [Header("Equipment Types")]
        public EquipmentType type;
        public ProjectileType projectileType;
    }
}
