using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class ShopSlotUI : InventorySlotUI
    {
        public ShopSlotUI(VisualElement root, Vector2 gridPosition,
            Func<Vector2, InventoryItemData, bool> itemObjectSpawningFunc) 
            : base(root, gridPosition, itemObjectSpawningFunc) 
        {
        
        }
    }
}
