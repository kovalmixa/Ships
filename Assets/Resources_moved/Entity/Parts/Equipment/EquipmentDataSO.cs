using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Handlers.Enums;
using Assets.Resources_moved.Entity.Parts;
using GameplayActions;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    [CreateAssetMenu(fileName = "NewEquipmentData", menuName = "Configs/Equipment Data")]
    public class EquipmentDataSO : ScriptableObject, IDataContainer
    {
        [Header("General Settings")]
        public GeneralOptions general;

        [Header("Equipment Types")]
        public EquipmentType type;
        public ProjectileType projectileType;

        public StatOptions statOptions;

        public LifeTimeData lifeTimeData;
    }
}
