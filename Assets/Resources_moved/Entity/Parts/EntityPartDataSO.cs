using Assets.Entity.StatMods;
using Assets.Resources_moved.Common.ContainerComponents;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Parts
{
    public class EntityPartDataSO : ScriptableObject, IInventorySlotData
    {
        [Header("General Settings")]
        [SerializeField] private GeneralOptions _general;
        public GeneralOptions General => _general;

        public StatOptions statOptions;

        public LifeCycleData lifeTimeData;

        private InventoryOptions _inventoryOptions;
        InventoryOptions IInventorySlotData.InventoryOptions => _inventoryOptions;

        public List<ModUnit> Mods => statOptions.mods;
    }
}
