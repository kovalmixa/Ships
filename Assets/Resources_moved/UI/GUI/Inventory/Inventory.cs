using Assets.Handlers.Math;
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

        [Header("Slot dimensions in pixels (must match the USS)")]
        [SerializeField] private float _slotSize = 64f;
        [SerializeField] private float _slotMargin = 4f;

        [Header("Default Colors (if not set in USS)")]
        [SerializeField] private Color _fallbackSlotColor = new Color(1f, 0.0f, 1f, 1f);

        private VisualElement _inventoryContainer;

        protected Dictionary<Vector2, SlotUI> slots = new();

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
            float totalSlotWidth = _slotSize + (_slotMargin * 2);
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
                slots.Add(new Vector2(x, y), GetSlot(slotVe));

            }
        }

        public SlotUI GetSlot(VisualElement ve)
        {
            Color baseColor = _fallbackSlotColor;
            Debug.Log(baseColor);

            if (ve.style.backgroundColor != null &&
                ve.style.backgroundColor.keyword != StyleKeyword.Undefined &&
                ve.style.backgroundColor.keyword != StyleKeyword.Null)
            {
                baseColor = ve.style.backgroundColor.value;
            }
            Color enterColor = ColorHandler.LightUpColor(baseColor);
            Debug.Log(baseColor);
            var config = new SlotUIConfig
            {
                onEnter = () => ve.style.backgroundColor = enterColor,
                onLeave = () => ve.style.backgroundColor = baseColor
            };

            var slot = new SlotUI();
            slot.Setup(ve, config);
            return slot;
        }
    }
}