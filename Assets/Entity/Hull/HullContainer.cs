using Assets.Common;
using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.DataContainers
{
    public class HullContainer : MonoBehaviour
    {
        public GeneralOptions general;
        public string Id { get; set; }
        [field: SerializeField] public VehicleSubType VehicleType { get; private set; }

        [SerializeField] public StatOptions statOptions;

    }
}