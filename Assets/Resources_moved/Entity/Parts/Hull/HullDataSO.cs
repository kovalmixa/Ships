using Assets.Handlers.Enums;
using Assets.Entity.Parts;
using UnityEngine;

namespace Assets.DataContainers
{
    [CreateAssetMenu(fileName = "NewHullData", menuName = "Configs/Hull Data")]
    public class HullDataSO : EntityPartDataSO
    {
        [Header("Vehicle Type")]
        public VehicleSubType vehicleType;
    }
}