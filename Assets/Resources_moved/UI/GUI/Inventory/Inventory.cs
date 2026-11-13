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
        [SerializeField] protected VisualTreeAsset slotItemPrefab;

        private VisualElement _inventoryVe;
        private VisualElement _slotsContainer;
        private VisualElement _itemsContainer;
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

            if (_inventoryVe != null)
            {
                _slotsContainer = _inventoryVe.Q<VisualElement>("SlotsContainer") ?? _inventoryVe;
                _itemsContainer = _inventoryVe.Q<VisualElement>("ItemsContainer");

                if (_itemsContainer == null)
                {
                    _itemsContainer = new VisualElement();
                    _itemsContainer.name = "ItemsContainer";
                    _itemsContainer.style.position = Position.Absolute;
                    _itemsContainer.style.left = 0;
                    _itemsContainer.style.top = 0;
                    _itemsContainer.style.right = 0;
                    _itemsContainer.style.bottom = 0;
                    _inventoryVe.Add(_itemsContainer);
                }

                BuildInventory(_gridWidth, _gridHeight);
            }
        }

        public void BuildInventory(int columns, int rows)
        {
            slots.Clear();
            _slotsContainer.Clear();
            _itemsContainer.Clear();

            var style = InventorySlotUI.SlotStyle;
            float totalSlotWidth = style.TotalSlotWidth;

            float paddingLeft = _inventoryVe.resolvedStyle.paddingLeft;
            float paddingRight = _inventoryVe.resolvedStyle.paddingRight;
            float paddingTop = _inventoryVe.resolvedStyle.paddingTop;
            float paddingBottom = _inventoryVe.resolvedStyle.paddingBottom;

            float gridWidth = (totalSlotWidth * columns) + (style.slotMargin * (columns - 1));
            float gridHeight = (totalSlotWidth * rows) + (style.slotMargin * (rows - 1));

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
                _slotsContainer.Add(slotVe);

                var slotElement = slotVe.Q<VisualElement>("SlotRoot") ?? slotVe.Q<VisualElement>();
                var slot = new InventorySlotUI(slotElement, new Vector2(x, y), TrySpawnItemUIWithSubInit);

                slots.Add(slot.GridPosition, slot);
            }
        }

        public bool TrySpawnItemUIWithSubInit(Vector2 startPosition, InventoryItemData itemData)
        {
            if (itemData == null) return false;
            if (slotItemPrefab == null)
            {
                Debug.LogError("slotItemPrefab не назначен в инвентаре!");
                return false;
            }

            var spanCols = itemData.inventoryOptions.SlotSize.x;
            var spanRows = itemData.inventoryOptions.SlotSize.y;
            if (spanCols == 0 || spanRows == 0)
            {
                Debug.LogError($"Slot size is 0,0! for {itemData.general.nameId}");
                return false;
            }

            for (int x = 0; x < spanCols; x++)
            {
                for (int y = 0; y < spanRows; y++)
                {
                    Vector2 pos = new Vector2(startPosition.x + x, startPosition.y + y);
                    if (!slots.TryGetValue(pos, out var targetSlot) 
                        || targetSlot.SlotItemData != null || targetSlot.hostSlot != null)
                        return false;
                }
            }

            var style = InventorySlotUI.SlotStyle;
            float cellSize = style.slotSize;
            float margin = style.slotMargin;
            float totalSlotWidth = style.TotalSlotWidth;

            float itemWidth = (spanCols * totalSlotWidth) - (margin * 2);
            float itemHeight = (spanRows * totalSlotWidth) - (margin * 2);

            float posX = startPosition.x * totalSlotWidth;
            float posY = startPosition.y * totalSlotWidth;

            var itemRoot = slotItemPrefab.CloneTree();
            var slotItemUI = new SlotItemUI(itemRoot);

            slotItemUI.SetPosition(posX, posY);
            slotItemUI.SetSize(itemWidth, itemHeight);

            _itemsContainer.Add(itemRoot);

            if (!slots.TryGetValue(startPosition, out var hostSlot)) return false;
            for (int x = 0; x < spanCols; x++)
            {
                for (int y = 0; y < spanRows; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector2 pos = new Vector2(startPosition.x + x, startPosition.y + y);
                    if (slots.TryGetValue(pos, out var subSlot))
                    {
                        subSlot.SetItem(itemData, itemData.quantity);
                        subSlot.hostSlot = hostSlot;
                        hostSlot.subSlots.Add(subSlot);
                    }
                }
            }
            return true;
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
            foreach (var slot in slots)
            {
                var slotUI = slot.Value;
                if (slotUI.SlotItemData != null && slotUI.hostSlot == null)
                    items[slot.Key] = slotUI.SlotItemData as InventoryItemData;
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
            foreach (var slot in slots)
            {
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