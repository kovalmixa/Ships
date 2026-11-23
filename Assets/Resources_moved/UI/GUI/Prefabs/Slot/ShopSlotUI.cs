using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class ShopSlotUI : InventorySlotUI
    {
        public ShopSlotUI(VisualElement root, Action<SlotUI> onClick, Vector2 gridPosition) : base(root, onClick, gridPosition) 
        {
        
        }
    }
}
