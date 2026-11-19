using Assets.Resources_moved.UI.GUI.Prefabs.Slot;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using Assets.Resources_moved.UI.UIToolkit;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private PaddingVector _slotPadding;
        private VisualElement _containersVe;
        private VisualElement _slotsContainer;
        private VisualElement _itemsContainer;
        private Label _inventoryLabel;

        protected Dictionary<Vector2, InventorySlotUI> slots = new();

        #region Setup

        public async UniTask SetupAsync(VisualElement root)
        {
            if (root != null)
            {
                _containersVe = root.Q<VisualElement>("Containers");
                _inventoryLabel = root.Q<Label>("Label");
            }

            if (_containersVe != null)
            {
                _slotsContainer = _containersVe.Q<VisualElement>("SlotsContainer");
                _itemsContainer = _containersVe.Q<VisualElement>("ItemsContainer");

                if (_itemsContainer == null)
                {
                    _itemsContainer = new VisualElement();
                    _itemsContainer.name = "ItemsContainer";
                    _itemsContainer.style.position = Position.Absolute;
                    _itemsContainer.style.left = 0;
                    _itemsContainer.style.top = 0;
                    _itemsContainer.style.right = 0;
                    _itemsContainer.style.bottom = 0;
                    _containersVe.Add(_itemsContainer);
                }

                await BuildInventoryAsync(_gridWidth, _gridHeight);
            }
        }

        public async UniTask BuildInventoryAsync(int columns, int rows, 
            CancellationToken cancellationToken = default)
        {
            slots.Clear();
            _slotsContainer.Clear();
            _itemsContainer.Clear();

            var style = InventorySlotUI.SlotStyle;
            float totalSlotWidth = style.TotalSlotWidth;
            PaddingVector inventroyVePadding = new PaddingVector(_containersVe);
            float gridWidth = (totalSlotWidth * columns) + (style.slotMargin * (columns - 1));
            float gridHeight = (totalSlotWidth * rows) + (style.slotMargin * (rows - 1));

            var slotContainerWidth = gridWidth + inventroyVePadding.Left + inventroyVePadding.Right;
            var slotContainerHeight = gridHeight + inventroyVePadding.Top + inventroyVePadding.Bottom;
            _containersVe.style.width = slotContainerWidth;
            _containersVe.style.height = slotContainerHeight;

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
                var slot = new InventorySlotUI(slotElement, new Vector2(x, y));

                slots.Add(slot.GridPosition, slot);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            // Или если нужно дождаться именно Layout Pass интерфейса:
            // await _slotsContainer.AwaitForNextPass().ToUniTask(cancellationToken: cancellationToken);

            var slotPaddingX = slotContainerWidth / columns - totalSlotWidth;
            var slotPaddingY = slotContainerHeight / rows - totalSlotWidth;

            _slotPadding = new PaddingVector(slotPaddingX, 0, slotPaddingY, 0);
            _slotPadding += new PaddingVector(_slotsContainer) + new MarginVector(_slotsContainer);

            Debug.Log($"Styles applied via UniTask! SlotPadding: {_slotPadding.Left}, {_slotPadding.Top}");
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
                if (slotUI.SlotItemData != null && slotUI.HostSlot == null)
                    items[slot.Key] = slotUI.SlotItemData as InventoryItemData;
            }
            return items;
        }

        #region Add item object

        public bool TryAddInventoryItem(Vector2 position, InventoryItemData data)
        {
            if (data == null || !slots.TryGetValue(position, out var slot)) return false;

            var host = slot.HostSlot ?? slot;
            if (!host.IsFree) return TryStack(host, data);
            return TryPlace(position, data);
        }

        public bool TryAddInventoryItem(InventoryItemData data)
        {
            foreach (var pos in slots.Keys)
                if (slots[pos].IsFree && TryPlace(pos, data)) return true;
            foreach (var slot in slots.Values)
                if (!slot.IsSubSlot && !slot.IsFree && TryStack(slot, data)) return true;
            return false;
        }

        #region Slot item logic

        private bool TryStack(InventorySlotUI host, InventoryItemData data)
        {
            if (!host.CompareItems(data)) return false;
            host.AddQuantity(data.quantity);
            return true;
        }

        private bool TryPlace(Vector2 start, InventoryItemData data)
        {
            if (!TryGetFootprint(start, data, out var cells)) return false;

            var size = data.inventoryOptions.SlotSize;
            var view = CreateItemView(start, (int)size.x, (int)size.y);
            _itemsContainer.Add(view.Root);
            cells[0].Occupy(data, view, cells.Skip(1));
            return true;
        }

        private bool TryGetFootprint(Vector2 start, InventoryItemData data, out List<InventorySlotUI> cells)
        {
            cells = new List<InventorySlotUI>();
            int cols = (int)data.inventoryOptions.SlotSize.x;
            int rows = (int)data.inventoryOptions.SlotSize.y;

            if (cols <= 0 || rows <= 0)
            {
                Debug.LogError($"Slot size is 0 for {data.general.nameId}");
                return false;
            }

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                {
                    var pos = new Vector2(start.x + x, start.y + y);
                    if (!slots.TryGetValue(pos, out var s) || !s.IsFree) return false;
                    cells.Add(s);
                }
            return true;
        }

        private SlotItemUI CreateItemView(Vector2 start, int cols, int rows)
        {
            var style = InventorySlotUI.SlotStyle;

            float total = style.TotalSlotWidth;
            float margin = style.slotMargin;
            var view = new SlotItemUI(slotItemPrefab.CloneTree());
            Vector2 localPos = _slotsContainer.WorldToLocal(slots[start].rootElement.localBound.position);

            float posX = (start.x * total) + _slotPadding.Left;
            float posY = (start.y * total) + _slotPadding.Top;
            Debug.Log($"{posX},{posY}");
            view.SetPosition(posX, posY);
            view.Size = new Vector2(cols * total - margin * 2, rows * total - margin * 2);
            return view;
        }

        #endregion

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
            if (!slots.TryGetValue(position, out var slot) || slot.IsFree) return false;
            slot.ClearSlot();
            return true;
        }

        public void RemoveAllItems()
        {
            foreach (var slot in slots) TryRemoveInventoryItemWithPosition(slot.Key);
        }

        #endregion

        #endregion
    }
}