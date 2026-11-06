using Assets.Entity;
using System;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data
{
    [Serializable]
    public class InventoryItemData : SlotItemData
    {
        public StatOptions statOptions;
        public int quantity;
    }
}