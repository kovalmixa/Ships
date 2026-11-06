using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources_moved.UI.GUI.Inventory.Player
{
    public class PlayerInventoryUI : UIWindow
    {
        [SerializeField] private Inventory _inventory;

        #region Setup

        protected override void Awake()
        {
            base.Awake();
            SetupPlayerInventory(PlayerInventoryController.Instance.InventoryItems);
        }

        public void SetupPlayerInventory(Dictionary<Vector2, InventoryItemData> slotsData)
        {
            foreach (var slot in slotsData)
            {
                if (!_inventory.TryAddInventoryItem(slot.Key, slot.Value))
                    Debug.LogWarning($"Unable to add item: {slot.Value.General.nameId} at Position: {slot.Key}");
            }
        }
        #endregion

        #region API



        #endregion
    }
}
