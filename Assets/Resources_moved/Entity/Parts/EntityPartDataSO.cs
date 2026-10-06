using Assets.Common;
using Assets.Common.Interfaces;
using UnityEngine;

namespace Assets.Entity.Parts
{
    public class EntityPartDataSO : ScriptableObject, IUIData
    {
        [Header("General Settings")]
        [SerializeField] private GeneralOptions _general;
        public GeneralOptions General { get => _general; }

        public StatOptions statOptions;

        public LifeTimeData lifeTimeData;
    }
}
