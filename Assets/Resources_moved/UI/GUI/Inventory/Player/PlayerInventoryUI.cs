using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Inventory.Player
{
    public class PlayerInventoryUI : UIWindow
    {
        [SerializeField] private Inventory _inventory;

        private PlayerInventoryController _inventoryController;

        #region Setup

        protected override void Awake()
        {
            _inventoryController = PlayerInventoryController.Instance;
            var root = RootVisualElement;
            var inventoryVe = root.Q<VisualElement>("Inventory");

            _inventory.Setup(inventoryVe);
            RegisterPointerCollisionForSingleElement(inventoryVe);
            _inventory.SetupInventoryItems(_inventoryController.InventoryItems);
            _inventoryController.SetPlayerInventoryUI(this);
        }

        #endregion

        #region API

        public Dictionary<Vector2, InventoryItemData> GetInventoryItems() => _inventory.GetInventoryItemsData();

        public bool TryAddItem(InventoryItemData itemData) => _inventory.TryAddInventoryItem(itemData);

        public bool TryRemoveItem(InventoryItemData itemData) => _inventory.TryRemoveItemWithData(itemData);

        #endregion
    }
}
