using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class InventorySlotUI : SlotUI
    {
        public InventorySlotUI hostSlot = null;
        public List<InventorySlotUI> subSlots = null;

        #region Setup

        #endregion

        #region API

        public override void SetItem(SlotItemData slotItemData)
        {
            // logic for multi-slot items
            base.SetItem(slotItemData);
        }

        public bool TryAddItem(InventoryItemData slotItemData)
        {
            var inventoryItemData = this.slotItemData as InventoryItemData;
            if (inventoryItemData == null) SetItem(slotItemData);
            else if (CompareItems(slotItemData)) inventoryItemData.quantity += slotItemData.quantity;
            else return false;
            return true;
        }

        public bool ChangeItem(InventoryItemData slotItemData)
        {
            var inventoryItemData = this.slotItemData as InventoryItemData;
            if (inventoryItemData == null) SetItem(slotItemData);
            else if (CompareItems(slotItemData)) inventoryItemData.quantity += slotItemData.quantity;
            else return false;
            return true;
        }

        public override bool CompareItems(SlotItemData slotItemData) => base.CompareItems(slotItemData) && this.slotItemData.stats == slotItemData.stats;

        #endregion
    }
}
