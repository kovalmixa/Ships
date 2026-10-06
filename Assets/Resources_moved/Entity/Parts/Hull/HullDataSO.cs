using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Handlers.Enums;
using Assets.Resources_moved.Entity.Parts;
using UnityEngine;

namespace Assets.DataContainers
{
    [CreateAssetMenu(fileName = "NewHullData", menuName = "Configs/Hull Data")]
    public class HullDataSO : ScriptableObject, IDataContainer
    {
        [Header("General Settings")]
        public GeneralOptions general;

        [Header("Vehicle Type")]
        public VehicleSubType vehicleType;

        public StatOptions statOptions;

        public LifeTimeData lifeTimeData;
    }
}