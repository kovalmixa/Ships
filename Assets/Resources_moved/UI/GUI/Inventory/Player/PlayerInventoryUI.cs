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
            _inventory.SetupInventoryItems(PlayerInventoryController.Instance.InventoryItems);
        }

        #endregion

        #region API



        #endregion
    }
}
