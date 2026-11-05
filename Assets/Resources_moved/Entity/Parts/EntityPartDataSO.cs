using Assets.Common.Interfaces;
using Assets.Resources_moved.Common.ContainerComponents;
using UnityEngine;

namespace Assets.Entity.Parts
{
    public class EntityPartDataSO : ScriptableObject, IItemDataContainer
    {
        [Header("General Settings")]
        [SerializeField] private GeneralOptions _general;
        public GeneralOptions General { get => _general; }

        public StatOptions statOptions;

        public LifeCycleData lifeTimeData;
    }
}
