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
        
        //reset comment if there is no way how
        //to get layer directly from prefab

        //public InterractLayerType layer;
    }
}