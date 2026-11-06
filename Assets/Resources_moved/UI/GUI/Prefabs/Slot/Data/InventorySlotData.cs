using Assets.Entity.StatMods;
using Assets.Resources_moved.Common.ContainerComponents;
using System;
using System.Collections.Generic;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data
{
    [Serializable]
    public class InventoryItemData : SlotItemData, IInventorySlotData
    {
        public InventoryOptions inventoryOptions;
        public List<ModUnit> mods;
        public int quantity;

        InventoryOptions IInventorySlotData.InventoryOptions => inventoryOptions;
        List<ModUnit> IInventorySlotData.Mods => mods;
    }
}