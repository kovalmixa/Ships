using Assets.Resources_moved.UI.Cursor;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using EventType = Assets.Resources_moved.UI.GUI.Prefabs.Slot.EventType;

public class InventorySlotUI : SlotUI
{
    private readonly List<InventorySlotUI> _subSlots = new();

    public InventorySlotUI HostSlot { get; private set; }
    public IReadOnlyList<InventorySlotUI> SubSlots => _subSlots;
    public Vector2 GridPosition { get; }
    public bool IsSubSlot => HostSlot != null;
    public bool IsFree => slotItemData == null && HostSlot == null;
    public InventoryItemData ItemData => slotItemData as InventoryItemData;

    #region Setup

    public InventorySlotUI(VisualElement root, Action<SlotUI> onClick, Vector2 gridPosition) : base(root, onClick) 
    {
        GridPosition = gridPosition;
    }

    protected virtual void SetColorWithSubs(Color color)
    {
        if (IsSubSlot && HostSlot == null) SetBackgroundColor(color);
        else if (IsSubSlot) HostSlot.SetColorWithSubs(color);
        else {
            SetBackgroundColor(color);
            foreach (var sub in SubSlots) sub.SetBackgroundColor(color);
        }
    }

    protected override void OnEnterEvent()
    {
        SetColorWithSubs(pointerEventColors[EventType.Enter]);
    }

    protected override void OnLeaveEvent()
    {
        SetColorWithSubs(pointerEventColors[EventType.Leave]);
    }

    protected override void OnDownEvent()
    {
        SetColorWithSubs(pointerEventColors[EventType.Down]);
        onClick?.Invoke(this);
    }

    protected override void OnPointerUpEvent()
    {
        SetColorWithSubs(pointerEventColors[EventType.Up]);
    }

    #endregion

    #region API

    public void Occupy(InventoryItemData data, SlotItemUI view, IEnumerable<InventorySlotUI> subs)
    {
        slotItemData = data;
        slotItemUI = view;
        view.ItemData = data;
        view.Number = data.quantity;

        foreach (var sub in subs)
        {
            sub.AttachTo(this);
            _subSlots.Add(sub);
        }
    }

    private void AttachTo(InventorySlotUI host)
    {
        HostSlot = host;
        slotItemData = host.slotItemData;
        slotItemUI = host.slotItemUI;
    }

    public void AddQuantity(int amount)
    {
        if (ItemData == null) return;
        ItemData.quantity += amount;
        slotItemUI.Number = ItemData.quantity;
    }

    public override void ClearSlot()
    {
        if (HostSlot != null) { HostSlot.ClearSlot(); return; }

        foreach (var sub in _subSlots)
        {
            sub.HostSlot = null;
            sub.slotItemData = null;
            sub.slotItemUI = null;
        }
        _subSlots.Clear();

        slotItemUI?.Root.RemoveFromHierarchy();
        slotItemUI = null;
        slotItemData = null;
    }

    public override void SetItem(SlotItemData data)
    {
        if (data == null) ClearSlot();
        else Debug.LogWarning("Inventory slots are filled via Inventory.TryAddInventoryItem");
    }

    public override bool CompareItem(IInventorySlotData other)
    {
        return base.CompareItem(other) && ItemData.mods == other.Mods;
    }

    #endregion
}