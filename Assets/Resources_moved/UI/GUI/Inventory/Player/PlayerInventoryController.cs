using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources_moved.UI.GUI.Inventory.Player
{
    public class PlayerInventoryController : SingletonMonoBehaviour<PlayerInventoryController>
    {
        private Dictionary<Vector2, InventoryItemData> _inventoryItems = new();

        public Dictionary<Vector2, InventoryItemData> InventoryItems => _inventoryItems;

        public event System.Action<Dictionary<Vector2, InventoryItemData>> OnInventoryUpdated;

        public void InitInventoryData(Dictionary<Vector2, InventoryItemData> loadedSlots)
        {
            _inventoryItems = loadedSlots ?? new Dictionary<Vector2, InventoryItemData>();
            OnInventoryUpdated?.Invoke(_inventoryItems);
        }

        #region API


        #endregion
    }
}