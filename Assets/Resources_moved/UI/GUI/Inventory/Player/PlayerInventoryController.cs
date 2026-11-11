using Assets.Global.FileGlobal;
using Assets.Global.SceneHandlers.GameSession;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources_moved.UI.GUI.Inventory.Player
{
    public class PlayerInventoryController : SingletonMonoBehaviour<PlayerInventoryController>
    {
        private Dictionary<Vector2, InventoryItemData> _inventoryItems = new();

        public Dictionary<Vector2, InventoryItemData> InventoryItems => _inventoryItems;

        private PlayerInventoryUI _playerInventoryUI;

        #region Setup

        public void SetPlayerInventoryUI(PlayerInventoryUI playerInventoryUI) => _playerInventoryUI = playerInventoryUI;

        public async UniTask InitInventoryData(List<InventorySlotDataSer> datas)
        {
            foreach (var data in datas)
            {
                string assetKey = data.itemId + "_data";
                ScriptableObject rawAsset = await PrefabLoader.Instance.LoadAssetAsync<ScriptableObject>(assetKey);

                if (rawAsset == null)
                {
                    Debug.LogWarning($"[InitInventory] Не удалось загрузить ассет по ключу: {assetKey}");
                    continue;
                }

                if (rawAsset is not IInventorySlotData slotData)
                {
                    Debug.LogWarning($"[InitInventory] Ассет '{assetKey}' (тип: {rawAsset.GetType().Name}) не реализует IInventorySlotData!");
                    continue;
                }

                var slotItem = BuildInventorySlot(slotData, data.quantity);
                _inventoryItems[data.position] = slotItem;
            }
        }

        private InventoryItemData BuildInventorySlot(IInventorySlotData isd, int quantity)
        {
            var inventorySlot = new InventoryItemData();
            inventorySlot.general = isd.General;
            inventorySlot.quantity = quantity;
            inventorySlot.mods = isd.Mods;
            inventorySlot.inventoryOptions = isd.InventoryOptions;
            return inventorySlot;
        }

        #endregion

        #region API

        public bool TryAddItem(InventoryItemData itemData)
        {
            if (_playerInventoryUI.TryAddItem(itemData))
            {
                _inventoryItems.Clear();
                _inventoryItems = _playerInventoryUI.GetInventoryItems();
                return true;
            }
            return false;
        }

        public bool TryRemoveItem(InventoryItemData itemData)
        {
            if (_playerInventoryUI.TryRemoveItem(itemData))
            {
                _inventoryItems.Clear();
                _inventoryItems = _playerInventoryUI.GetInventoryItems();
                return true;
            }
            return false;
        }

        #endregion
    }
}