using System.Collections.Generic;
using Assets.Global.FileGlobal;
using Assets.Global.Math;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class SlotStyle
    {
        public Color slotBackgroundColor = new(0f, 0f, 0f, 0.4f);
        public float slotSize = 64;
        public float slotMargin = 2f;

        public float TotalSlotWidth => slotSize + slotMargin * 2;
    }

    public enum EventType { Enter, Leave, Down, Up }

    public class SlotUI
    {
        protected Image backgroundImage;
        protected VisualTreeAsset slotItemPrefab;
        protected SlotItemData slotItemData = null;
        protected Dictionary<EventType, Color> pointerEventColors = new();

        public SlotItemUI slotItemUI;
        public VisualElement rootElement;

        public SlotItemData SlotItemData => slotItemData;

        public static SlotStyle SlotStyle => new SlotStyle();

        #region Setup

        public SlotUI(VisualElement root)
        {
            rootElement = root;
            backgroundImage = rootElement.Q<Image>("BackgroundImage");
            var slotItemPrefab = PrefabLoader.Instance.LoadAssetSync<VisualTreeAsset>("SlotItem");
            
            if (backgroundImage == null) return;
            SetupFillColors();
            backgroundImage.RegisterCallback<PointerEnterEvent>(evt => OnEnterEvent());
            backgroundImage.RegisterCallback<PointerLeaveEvent>(evt => OnLeaveEvent());
            backgroundImage.RegisterCallback<PointerDownEvent>(evt => OnDownEvent());
            backgroundImage.RegisterCallback<PointerUpEvent>(evt => OnPointerUpEvent());
        }

        protected virtual void SetupFillColors()
        {
            var enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            var downColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 1f);
            var leaveColor = SlotStyle.slotBackgroundColor;
            var upColor = enterColor;

            SetupEventCollorFill(EventType.Enter, enterColor);
            SetupEventCollorFill(EventType.Leave, leaveColor);
            SetupEventCollorFill(EventType.Down, downColor);
            SetupEventCollorFill(EventType.Up, upColor);
        }

        private void SetupEventCollorFill(EventType type, Color color)
        {
            if (pointerEventColors.ContainsKey(type)) pointerEventColors[type] = color;
            else pointerEventColors.Add(type, color);
        }

        protected virtual void SetBackgroundColor(Color newColor)
        {
            if (backgroundImage == null) return;
            backgroundImage.style.backgroundColor = newColor;
        }

        protected virtual void OnEnterEvent() => SetBackgroundColor(pointerEventColors[EventType.Enter]);

        protected virtual void OnLeaveEvent() => SetBackgroundColor(pointerEventColors[EventType.Leave]);

        protected virtual void OnDownEvent() => SetBackgroundColor(pointerEventColors[EventType.Down]);

        protected virtual void OnPointerUpEvent() => SetBackgroundColor(pointerEventColors[EventType.Up]);

        #endregion

        #region API

        public virtual void SetItem(SlotItemData newItemData)
        {
            slotItemData = newItemData;
            if (slotItemUI == null && newItemData != null) SpawnSlotItemUI();
            if (slotItemUI != null)
            {
                if (newItemData == null) ClearSlot();
                else slotItemUI.ItemData = newItemData;
            }
        }

        private void SpawnSlotItemUI()
        {
            if (slotItemPrefab != null)
            {
                var itemRoot = slotItemPrefab.CloneTree();
                slotItemUI = new SlotItemUI(itemRoot);
            }
            else Debug.LogWarning("Unable to find slot item prefab");
        }

        public virtual void ClearSlot()
        {
            if (slotItemUI == null) return;
            slotItemUI = null;
        }

        public virtual bool CompareItem(IInventorySlotData other)
        {
            return slotItemData.general.nameId == other.General.nameId;
        }

        #endregion
    }
}