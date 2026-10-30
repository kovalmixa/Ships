using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("Size of grid in slot units")]
        [SerializeField] private int _gridWidth = 5;
        [SerializeField] private int _gridHeight = 4;

        [Header("Slot dimensions in pixels (must match the USS)")]
        [SerializeField] private float _slotSize = 64f;
        [SerializeField] private float _slotMargin = 4f;

        private VisualElement _inventoryContainer;

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _inventoryContainer = root.Q<VisualElement>("InventoryContainer");
            if (_inventoryContainer != null) BuildInventory(_gridWidth, _gridHeight);
        }

        public void BuildInventory(int columns, int rows)
        {
            _inventoryContainer.Clear();
            float totalSlotWidth = _slotSize + (_slotMargin * 2);
            float containerWidth = (totalSlotWidth * columns)
                + _inventoryContainer.resolvedStyle.paddingLeft
                + _inventoryContainer.resolvedStyle.paddingRight;

            _inventoryContainer.style.width = (totalSlotWidth * columns) + 16;

            int totalSlots = columns * rows;
            for (int i = 0; i < totalSlots; i++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList("inventory-slot");
                int x = i % columns;
                int y = i / columns;
                slot.name = $"Slot_{x}_{y}";
                _inventoryContainer.Add(slot);
            }
        }
    }
}