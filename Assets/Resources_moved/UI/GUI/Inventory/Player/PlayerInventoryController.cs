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

        public event System.Action<Dictionary<Vector2, InventoryItemData>> OnInventoryUpdated;

        public async UniTask InitInventoryData(List<InventorySlotDataSer> datas)
        {
            foreach(var data in datas)
            {
                var prefab = await PrefabLoader.Instance.GetPrefabAsync(data.itemId);
                if (prefab == null) continue;
                var isd = prefab.GetComponent<IInventorySlotData>();
                if (isd == null) continue;

                var slotItem = BuildInventorySlot(isd, data.quantity);
                _inventoryItems[data.position] = slotItem;
            }

            OnInventoryUpdated?.Invoke(_inventoryItems);
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

        #region API


        #endregion
    }
}