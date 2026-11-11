using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class InventorySlotUI : SlotUI
    {
        public InventorySlotUI hostSlot = null;
        public List<InventorySlotUI> subSlots = null;

        public InventorySlotUI(VisualElement root) : base(root) { }

        protected override void SetupPointerDownEvent()
        {
            base.SetupPointerDownEvent();
            // логика для специфичного нажатия на слот инвентаря
        }

        public void SetItem(SlotItemData slotItemData, int number = 1)
        {
            base.SetItem(slotItemData);
            if (slotItemUI != null) slotItemUI.Number = slotItemData == null ? 0 : number;
        }

        public bool TryAddItem(InventoryItemData incomingItemData)
        {
            var currentInventoryItem = slotItemData as InventoryItemData;

            if (currentInventoryItem == null)
                SetItem(incomingItemData, incomingItemData.quantity);
            else if (CompareItems(incomingItemData))
            {
                currentInventoryItem.quantity += incomingItemData.quantity;
                SetItem(currentInventoryItem, currentInventoryItem.quantity);
            }
            else return false;
            return true;
        }

        public override bool CompareItems(SlotItemData slotItemData)
        {
            var otherInventoryData = slotItemData as IInventorySlotData;
            var thisInventoryData = this.slotItemData as IInventorySlotData;

            if (otherInventoryData == null || thisInventoryData == null)
                return base.CompareItems(slotItemData);

            return base.CompareItems(slotItemData) && thisInventoryData.Mods.SequenceEqual(otherInventoryData.Mods);
        }
    }
}