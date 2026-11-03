using UnityEngine;

namespace Assets.Resources_moved.UI.GUI.Inventory.Player
{
    public class PlayerInventory : UIWindow
    {
        [SerializeField] private Inventory _inventory;

        #region Setup

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion
    }
}
