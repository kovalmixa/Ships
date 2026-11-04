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

        public override void SetItem(SlotItemData data)
        {
            // logic for multi-slot items
            base.SetItem(data);
        }

        public bool TryAddItem(InventoryItemData data)
        {
            var inventoryItemData = slotItemData as InventoryItemData;
            if (inventoryItemData == null) SetItem(data);
            else if (inventoryItemData.modName == data.modName) inventoryItemData.quantity += data.quantity;
            else return false;
            return true;
        }

        public bool ChangeItem(InventoryItemData data)
        {
            var inventoryItemData = slotItemData as InventoryItemData;
            if (inventoryItemData == null) SetItem(data);
            else if (inventoryItemData.modName == data.modName) inventoryItemData.quantity += data.quantity;
            else return false;
            return true;
        }

        #endregion

    }
}
