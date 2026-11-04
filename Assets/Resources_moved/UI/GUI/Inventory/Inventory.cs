using Assets.Resources_moved.UI.GUI.Prefabs.Slot;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Inventory
{
    public class Inventory : UIWindow
    {
        [Header("Size of grid in slot units")]
        [SerializeField] private int _gridWidth = 5;
        [SerializeField] private int _gridHeight = 4;

        private VisualElement _inventoryContainer;
        protected Dictionary<Vector2, InventorySlotUI> slots = new();

        #region Setup

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _inventoryContainer = root.Q<VisualElement>("Inventory");
            if (_inventoryContainer != null) BuildInventory(_gridWidth, _gridHeight);
        }

        public void BuildInventory(int columns, int rows)
        {
            slots.Clear();
            _inventoryContainer.Clear();

            float paddingLeft = _inventoryContainer.resolvedStyle.paddingLeft;
            float paddingRight = _inventoryContainer.resolvedStyle.paddingRight;
            float totalSlotWidth = SlotUI.SlotStyle.slotSize + (SlotUI.SlotStyle.slotMargin * 2);
            float gridWidth = (totalSlotWidth * columns) + (2 * (columns - 1));
            _inventoryContainer.style.width = gridWidth + paddingLeft + paddingRight;

            int totalSlots = columns * rows;
            for (int i = 0; i < totalSlots; i++)
            {
                VisualElement slotVe = new VisualElement();
                slotVe.AddToClassList("inventory-slot");
                int x = i % columns;
                int y = i / columns;
                slotVe.name = $"Slot_{x}_{y}";
                _inventoryContainer.Add(slotVe);
                RegisterPointerEventForElement(slotVe);

                var slot = new InventorySlotUI();
                slot.Setup(slotVe);
                slots.Add(new Vector2(x, y), slot);
            }
        }

        public void SetupInventoryItems(Dictionary<Vector2, InventoryItemData> itemsDict)
        {
            foreach (var kpv in itemsDict) TryAddInventoryItem(kpv.Key, kpv.Value);
        }

        #endregion

        #region API

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