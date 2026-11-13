using Assets.Global.FileGlobal;
using Assets.Global.Math;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class SlotStyle
    {
        public Color slotBackgroundColor = new(0f, 0f, 0f, 0.4f);
        public float slotSize = 64;
        public float slotMargin = 2f;

        public float TotalSlotWidth => slotSize + slotMargin * 2;
    }

    public class SlotUI
    {
        protected Image backgroundImage;
        protected SlotItemData slotItemData = null;
        protected VisualTreeAsset slotItemPrefab;

        public SlotItemData SlotItemData => slotItemData;
        public SlotItemUI slotItemUI;
        public VisualElement rootElement;

        public static SlotStyle SlotStyle => new SlotStyle();

        #region Setup

        public SlotUI(VisualElement root)
        {
            rootElement = root;
            backgroundImage = rootElement.Q<Image>("BackgroundImage");
            var slotItemPrefab = PrefabLoader.Instance.LoadAssetSync<VisualTreeAsset>("SlotItem");

            SetupEnterEvent();
            SetupLeaveEvent();
            SetupPointerDownEvent();
            SetupClickPointerUpEvent();
        }

        protected virtual void SetupEnterEvent()
        {
            if (backgroundImage == null) return;
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            backgroundImage.RegisterCallback<PointerEnterEvent>(evt => backgroundImage.style.backgroundColor = enterColor);
        }

        protected virtual void SetupLeaveEvent()
        {
            if (backgroundImage == null) return;
            backgroundImage.RegisterCallback<PointerLeaveEvent>(evt => backgroundImage.style.backgroundColor = SlotStyle.slotBackgroundColor);
        }

        protected virtual void SetupPointerDownEvent()
        {
            if (backgroundImage == null) return;
            Color clickColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 1f);
            backgroundImage.RegisterCallback<PointerDownEvent>(evt => backgroundImage.style.backgroundColor = clickColor);
        }

        protected virtual void SetupClickPointerUpEvent()
        {
            if (backgroundImage == null) return;
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            backgroundImage.RegisterCallback<PointerUpEvent>(evt => backgroundImage.style.backgroundColor = enterColor);
        }

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

        protected virtual void SpawnSlotItemUI()
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

        public virtual bool CompareItems(SlotItemData otherData) => slotItemData == otherData;

        #endregion
    }
}