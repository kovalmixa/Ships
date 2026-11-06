using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class InventorySlotUI : SlotUI
    {
        public InventorySlotUI hostSlot = null;
        public List<InventorySlotUI> subSlots = null;

        #region Setup

        protected override void SetupPointerDownEvent()
        {
            base.SetupPointerDownEvent();

            // logic for setting pointer
        }

        #endregion

        #region API

        public override void SetItem(SlotItemData slotItemData, int count = 1)
        {
            // логика для мульти-слотовых предметов, если требуется
            base.SetItem(slotItemData, count);
        }

        public bool TryAddItem(InventoryItemData incomingItemData)
        {
            var currentInventoryItem = slotItemData as InventoryItemData;

            if (currentInventoryItem == null) SetItem(incomingItemData, incomingItemData.quantity);
            else if (CompareItems(incomingItemData))
            {
                currentInventoryItem.quantity += incomingItemData.quantity;
                SetItem(currentInventoryItem, currentInventoryItem.quantity);
            }
            else return false;
            return true;
        }

        public bool ChangeItem(InventoryItemData incomingItemData)
        {
            var currentInventoryItem = slotItemData as InventoryItemData;

            if (currentInventoryItem == null) SetItem(incomingItemData, incomingItemData.quantity);
            else if (CompareItems(incomingItemData))
            {
                currentInventoryItem.quantity += incomingItemData.quantity;
                SetItem(currentInventoryItem, currentInventoryItem.quantity);
            }
            else return false;
            return true;
        }

        public override bool CompareItems(SlotItemData slotItemData) =>
            base.CompareItems(slotItemData) && this.slotItemData.stats == slotItemData.stats;

        #endregion
    }
}