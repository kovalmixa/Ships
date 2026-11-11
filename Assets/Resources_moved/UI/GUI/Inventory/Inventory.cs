using Assets.Resources_moved.UI.GUI.Prefabs.Slot;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Inventory
{
    [Serializable]
    public class Inventory
    {
        [Header("Slot options")]
        [SerializeField] private int _gridWidth = 5;
        [SerializeField] private int _gridHeight = 4;
        [SerializeField] protected VisualTreeAsset slotPrefab;

        private VisualElement _inventoryVe;
        private Label _inventoryLabel;
        protected Dictionary<Vector2, InventorySlotUI> slots = new();

        #region Setup

        public void Setup(VisualElement root)
        {
            if (root != null)
            {
                _inventoryVe = root.Q<VisualElement>("Container");
                _inventoryLabel = root.Q<Label>("Label");
            }
            if (_inventoryVe != null) BuildInventory(_gridWidth, _gridHeight);
        }

        public void BuildInventory(int columns, int rows)
        {
            slots.Clear();
            _inventoryVe.Clear();

            float paddingLeft = _inventoryVe.resolvedStyle.paddingLeft;
            float paddingRight = _inventoryVe.resolvedStyle.paddingRight;
            float paddingTop = _inventoryVe.resolvedStyle.paddingRight;
            float paddingBottom = _inventoryVe.resolvedStyle.paddingRight;
            float totalSlotWidth = SlotUI.SlotStyle.TotalSlotWidth;

            float gridWidth = (totalSlotWidth * columns) + (SlotUI.SlotStyle.slotMargin * (columns - 1));
            float gridHeight = (totalSlotWidth * rows) + (SlotUI.SlotStyle.slotMargin * (rows - 1));

            _inventoryVe.style.width = gridWidth + paddingLeft + paddingRight;
            _inventoryVe.style.height = gridHeight + paddingTop + paddingBottom;

            int totalSlots = columns * rows;
            for (int i = 0; i < totalSlots; i++)
            {
                var slotVe = slotPrefab.CloneTree();
                slotVe.AddToClassList("inventory-slot");
                int x = i % columns;
                int y = i / columns;
                slotVe.name = $"Slot_{x}_{y}";
                _inventoryVe.Add(slotVe);

                var slotElement = slotVe.Q<VisualElement>("SlotRoot") ?? slotVe.Q<VisualElement>();
                var slot = new InventorySlotUI(slotElement);

                slots.Add(new Vector2(x, y), slot);
            }
        }

        public void SetupInventoryItems(Dictionary<Vector2, InventoryItemData> itemsDict)
        {
            RemoveAllItems();
            foreach (var kpv in itemsDict)
            {
                if (!TryAddInventoryItem(kpv.Key, kpv.Value))
                    Debug.LogWarning($"Unable to add item: {kpv.Value.General.nameId} at Position: {kpv.Key}");
            }
        }

        #endregion

        #region API

        public void SetName(string name) => _inventoryLabel.text = name;

        public Dictionary<Vector2, InventoryItemData> GetInventoryItemsData()
        {
            Dictionary<Vector2, InventoryItemData> items = new();
            InventoryItemData tmpData = null;
            foreach (var slot in slots)
            {
                tmpData = slot.Value.SlotItemData as InventoryItemData;
                if (tmpData != null) items[slot.Key] = tmpData;
            }
            return items;
        }

        #region Add item object

        public bool TryAddInventoryItem(Vector2 position, InventoryItemData itemData)
        {
            if (slots.TryGetValue(position, out var slot)) return slot.TryAddItem(itemData);
            return false;
        }

        public bool TryAddInventoryItem(InventoryItemData itemData)
        {
            foreach (var position in slots.Keys) if (TryAddInventoryItem(position, itemData)) return true;
            return false;
        }

        #endregion

        #region Remove item

        public bool TryRemoveItemWithData(InventoryItemData data)
        {
            foreach (var slot in slots)
            {
                if (slot.Value.CompareItems(data))
                {
                    slot.Value.SetItem(null);
                    return true;
                }
            }
            return false;
        }

        public bool TryRemoveInventoryItemWithPosition(Vector2 position)
        {
            if (slots.TryGetValue(position, out var slot))
            {
                slot.SetItem(null);
                return true;
            }
            return false;
        }

        public void RemoveAllItems()
        {
            foreach (var slot in slots) {
                //logic with processing dimentions of items and sparce movements
                TryRemoveInventoryItemWithPosition(slot.Key);
            }
        }

        #endregion

        #region Change item 

        //public bool TryChangeItem(Vector2 position, InventoryItemData itemData)
        //{
        //    if (slots.TryGetValue(position, out var slot))
        //    {
        //        InventoryItemData slotItem = slot.SlotItemData as InventoryItemData;
        //        if (slotItem == null) slot.SlotItemData = itemData;
        //        else if (slotItem.modName == itemData.modName) slotItem.quantity++; // add cap for stack
        //        else return false;
        //        return true;
        //    }
        //}

        #endregion

        #endregion
    }
}