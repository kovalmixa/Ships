using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class InventorySlotUI : SlotUI
    {
        public InventorySlotUI hostSlot = null;
        public List<InventorySlotUI> subSlots = new();
        private Func<InventoryItemData, bool> onItemSetup;

        public bool IsSubSlot => hostSlot != null;
        public Vector2 GridPosition { get; set; }

        #region Setup

        public InventorySlotUI(VisualElement root, Vector2 gridPosition,
            Func<Vector2, InventoryItemData, bool> itemObjectSpawningFunc) : base(root)
        {
            GridPosition = gridPosition;
            onItemSetup = (itemData) => itemObjectSpawningFunc(GridPosition, itemData);
        }

        protected override void SetupPointerDownEvent()
        {
            base.SetupPointerDownEvent();
        }

        #endregion

        #region API

        public override bool CompareItems(SlotItemData slotItemData)
        {
            var otherInventoryData = slotItemData as IInventorySlotData;
            var thisInventoryData = this.slotItemData as IInventorySlotData;

            if (otherInventoryData == null || thisInventoryData == null)
                return base.CompareItems(slotItemData);

            return base.CompareItems(slotItemData) && thisInventoryData.Mods.SequenceEqual(otherInventoryData.Mods);
        }

        #region SetItem

        public override void SetItem(SlotItemData slotItemData) => SetItem(slotItemData, 1);

        public void SetItem(SlotItemData slotItemData, int number)
        {
            base.SetItem(slotItemData);
            if (slotItemUI != null) slotItemUI.Number = slotItemData == null ? 0 : number;
        }

        protected override void SpawnSlotItemUI()
        {
            if (IsSubSlot) return;
            if (!onItemSetup(slotItemData as InventoryItemData)) 
                Debug.LogWarning("Unable to set item data");
        }

        #endregion

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

        public override void ClearSlot()
        {
            if (slotItemUI == null) return;
            if (hostSlot != null)
            {
                hostSlot.ClearSlot();
                return;
            }

            if (subSlots != null && subSlots.Count > 0)
            {
                foreach (var subSlot in subSlots)
                {
                    if (subSlot != null)
                    {
                        subSlot.slotItemData = null;
                        subSlot.hostSlot = null;
                        subSlot.slotItemUI = null;
                    }
                }
                subSlots.Clear();
            }

            SetItem(null, 0);
            slotItemUI = null;
        }

        #endregion
    }
}