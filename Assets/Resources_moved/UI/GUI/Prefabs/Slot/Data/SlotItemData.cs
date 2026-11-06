using Assets.Common.Interfaces;
using Assets.Entity;
using Assets.Resources_moved.Common.ContainerComponents;
using System;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data
{
    [Serializable]
    public class SlotItemData : IItemDataContainer
    {
        public InventoryOptions inventoryOptions;
        public StatOptions stats;
        public GeneralOptions general;

        public GeneralOptions General => general;
    }
}
